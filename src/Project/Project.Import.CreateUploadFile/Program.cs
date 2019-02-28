using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Project.Import.CreateUploadFile
{
    class Program
    {
        private const string CatalogName = "BBQG"; // "Habitat_Master"; 
        static void Main(string[] args)
        {
            try
            {
                var categoryList = new Dictionary<string, Category>();
                var productList = new Dictionary<string, Product>();

                GetCategoryhierarchy(categoryList);
                GetCategoryToProductAssociation(categoryList, productList);
                //GetCategories();
                GetProducts(productList);
                CleanProductCategories(productList);
                GetImages(productList);
                CreateFile(productList);
                CreateFile(categoryList);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.ReadLine();
            }
        }

        private static void GetCategoryhierarchy(Dictionary<string, Category> categoryList)
        {
            var web = new HtmlWeb();
            var doc = web.Load(Sites.BBQG.Config.Retrieve(Sites.BBQG.Config.Url));

            var nav = doc.DocumentNode.SelectSingleNode("//nav[@id='nav']");

            GetRootCategories(nav, categoryList);
        }

        private static void GetCategoryToProductAssociation(Dictionary<string, Category> categoryList, Dictionary<string, Product> productList)
        {
            foreach (var category in categoryList.Values)
            {
                var url = category.Url;
                url = (url.StartsWith("/")) ? Sites.BBQG.Config.Retrieve(Sites.BBQG.Config.Url) + url : url;

                GetCategoryToProductAssociationByPage(productList, category, url);
            }

            categoryList.Values.Where(c => c.ProductIdList.Count() == 0).ToList().ForEach(c => categoryList.Remove(c.Id));
        }

        private static void GetCategoryToProductAssociationByPage(Dictionary<string, Product> productList, Category category, string url)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Retrieving product from page " + url);

            if (string.IsNullOrEmpty(url)) return;

            var web = new HtmlWeb();
            var doc = web.Load(url);

            var productNodeList = doc.DocumentNode.SelectNodes("//ul[starts-with(@class, 'products-grid')]/li[@class='item']");

            if (productNodeList == null) return;

            foreach (var product in productNodeList)
            {

                var productId = product.SelectSingleNode(".//div[@class='trustpilot-widget']").Attributes["data-sku"].Value;
                var node = product.SelectSingleNode(".//h2[@class='product-name']/a");
                var productUrl = node.Attributes["href"];
                var displayName = node.Attributes["title"].Value;

                Product.AddUpdate(productList, category, productId, displayName, productUrl, url);
            }

            var pagerNodeList = doc.DocumentNode.SelectNodes("//div[@class='pages']/ol/li");

            if (pagerNodeList == null) return;

            // will only move to the next page if the last page isn't the current page.
            for (int i = 0; i < pagerNodeList.Count() - 1; i++)
            {
                if ((pagerNodeList[i].Attributes["class"]?.Value.Equals("current")).GetValueOrDefault())
                {
                    var nextpageUrl = pagerNodeList[i + 1].SelectSingleNode(".//a").Attributes["href"].Value;
                    GetCategoryToProductAssociationByPage(productList, category, nextpageUrl);
                    break;
                }
            }
        }

        private static void GetRootCategories(HtmlNode startNode, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes(".//ol/ul/li[starts-with(@class, 'level0')]");

            foreach (var categoryNode in categoryNodeList)
            {
                var node = categoryNode.SelectSingleNode(".//a[contains(@class, 'level0')]");

                Console.WriteLine();
                Console.WriteLine($"***************************************************************************************");

                var category = Category.Add(categoryList, "", node.InnerHtml, node.Attributes["href"]);
                GetLevel1Categories(categoryNode, category, categoryList);
            }
        }

        private static void GetLevel1Categories(HtmlNode startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes(".//div[starts-with(@class, 'level1')]");

            if (categoryNodeList == null) return;
            foreach (var categoryNode in categoryNodeList)
            {
                var node = categoryNode.SelectSingleNode(".//a[contains(@class, 'level1')]");

                if (node == null) continue;

                var category = Category.Add(categoryList, parentCategory.Id, node.InnerHtml, node.Attributes["href"]);
                GetLevel2Categories(categoryNode, category, categoryList);
            }
        }

        private static void GetLevel2Categories(HtmlNode startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes(".//li[starts-with(@class, 'level2')]");

            if (categoryNodeList == null) return;
            foreach (var categoryNode in categoryNodeList)
            {
                var node = categoryNode.SelectSingleNode(".//a");

                Category.Add(categoryList, parentCategory.Id, node.InnerHtml, node.Attributes["href"]);
            }
        }

        private static void GetProducts(Dictionary<string, Product> productList)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Getting further product details");

            foreach (var product in productList.Values)
            {
                Console.WriteLine("UPDATING " + product.ToString());

                var url = product.Url;
                url = (url.StartsWith("/")) ? Sites.BBQG.Config.Retrieve(Sites.BBQG.Config.Url) + url : url;

                var web = new HtmlWeb();
                var doc = web.Load(url);

                product.Price = doc.DocumentNode.SelectSingleNode("//span[@class='price']")?.InnerHtml;
                if (product.Price == null)
                    return;
                product.Price = product.Price.Remove(0, product.Price.LastIndexOf('>') + 1).Replace(",", "").TrimEnd();

                var imageNodeList = doc.DocumentNode.SelectNodes("//div[starts-with(@class, 'main-image-set')]/div/img");
                foreach (var imageNode in imageNodeList)
                {
                    product.ImageUrlList.Add(imageNode.Attributes["src"].Value);
                }
            }

            productList.Values.Where(p => p.Price == null).ToList().ForEach(c => productList.Remove(c.Id));
        }

        private static void CleanProductCategories(Dictionary<string, Product> productList)
        {
            // Comment or uncomment this method depending if you want to a product to only be contained in the end leaves of the category tree.
            foreach (var product in productList.Values)
            {
                var removeList = new List<string>();
                foreach (var category in product.CategoryIdList)
                {
                    if (product.CategoryIdList.Any(c => c != category && c.StartsWith(category)))
                    {
                        removeList.Add(category);
                    }
                }
                product.CategoryIdList = product.CategoryIdList.Except(removeList).ToList();
            }
        }

        private static void GetImages(Dictionary<string, Product> productList)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Getting product images");

            var directoryLocation = @"c:\Import\Images";
            var directoryInfo = Directory.CreateDirectory(directoryLocation);

            using (var webClient = new WebClient())
            {
                foreach (var product in productList.Values)
                {
                    for (int i = 0; i < product.ImageUrlList.Count(); i++)
                    {
                        var url = product.ImageUrlList.ElementAt(i);
                        url = (url.StartsWith("/")) ? Sites.BBQG.Config.Retrieve(Sites.BBQG.Config.Url) + url : url;

                        var extensionStartIndex = url.LastIndexOf('.');
                        var fileName = $"{product.Id}_{i}{url.Substring(extensionStartIndex, url.Length - extensionStartIndex)}";

                        Console.WriteLine($"Downloading: '{url}' to '{fileName}'");
                        webClient.DownloadFile(url, Path.Combine(directoryLocation, fileName));
                        product.ImageNameList.Add(fileName);

                        // For the moment, only get 1 image per product
                        break;
                    }
                }
            }
        }

        private static void CreateFile(Dictionary<string, Product> productList)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Creating File");

            var fileName = $"ProductImport_{DateTime.Now.ToString("yyyyMMdd")}";
            var directoryLocation = @"c:\Import";
            var filePath = Path.Combine(directoryLocation, fileName + ".csv");
            var directoryInfo = Directory.CreateDirectory(directoryLocation);

            using (var file = File.CreateText(filePath))
            {
                var headerList = new List<string>
                {
                    "ProductId", // 0
                    "ProductName", // 1
                    "DisplayName", // 2
                    "Description", // 3
                    "Brand", // 4
                    "Manufacturer", // 5
                    "TypeOfGood", // 6
                    "Tags", // 7
                    "ListPrice", // 8
                    "Images", // 9
                    "CatalogName", // 10
                    "CategoryName", // 11
                    "Style", // 12
                    "FuelType", // 13
                    "NaturalGasConversionAvailable", // 14
                    "DimensionsHeightHoodOpen", // 15
                    "DimensionsHeightHoodClosed", // 16
                    "DimensionsWidth", // 17
                    "DimensionsDepth", // 18
                };

                file.WriteLine(string.Join(',', headerList));

                var line = new StringBuilder();
                foreach (var product in productList.Values)
                {
                    line.Clear();
                    line.Append(CatalogName + product.Id + ","); //"ProductId", // 0
                    line.Append(product.DisplayName?.StringToCSVCell() + ","); //"ProductName", // 1
                    line.Append(product.DisplayName?.StringToCSVCell() + ",");//"DisplayName", // 2
                    line.Append(product.Description?.StringToCSVCell() + ","); //"Description", // 3
                    line.Append(",");//"Brand", // 4
                    line.Append(",");//"Manufacturer", // 5
                    line.Append(",");//"TypeOfGood", // 6
                    line.Append(",");//"Tags", // 7
                    line.Append("USD-" + product.Price + ",");//"ListPrice", // 8
                    line.Append(string.Join('|', product.ImageNameList) + ",");//"Images", // 9
                    line.Append(CatalogName + ",");//"CatalogName", // 10
                    product.CategoryIdList.ForEach(c => line.Append(string.Join("^^", $"{CatalogName}^-{c}"))); //"CategoryName", // 11
                    line.Append(",");//"CategoryName", // 11
                    line.Append(",");//"Style", // 12
                    line.Append(",");//"FuelType", // 13
                    line.Append(",");//"NaturalGasConversionAvailable", // 14
                    line.Append(",");//"DimensionsHeightHoodOpen", // 15
                    line.Append(",");//"DimensionsHeightHoodClosed", // 16
                    line.Append(",");//"DimensionsWidth", // 17
                    //line.Append();//"DimensionsDepth", // 18

                    file.WriteLine(line);
                }
            }

            File.Move(filePath, Path.Combine(directoryLocation, fileName + ".CSV"));
        }

        private static void CreateFile(Dictionary<string, Category> categoryList)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Creating File");

            var fileName = $"CategoryImport_{DateTime.Now.ToString("yyyyMMdd")}";
            var directoryLocation = @"c:\Import";
            var filePath = Path.Combine(directoryLocation, fileName + ".csv");
            var directoryInfo = Directory.CreateDirectory(directoryLocation);

            using (var file = File.CreateText(filePath))
            {
                var headerList = new List<string>
                {
                    "CatalogName", // 0
                    "CategoryName", // 1
                    "ParentCategoryName", // 2
                    "CategoryDisplayName", // 3
                };

                file.WriteLine(string.Join(',', headerList));

                var line = new StringBuilder();
                foreach (var category in categoryList.Values)
                {
                    line.Clear();
                    line.Append(CatalogName + ","); //"CatalogName", // 0
                    line.Append(category.Id + ","); //"CategoryName", // 1
                    line.Append(category.ParentCategoryId + ",");//"ParentCategoryName", // 2
                    line.Append(category.DisplayName); //"DisplayName", // 3

                    file.WriteLine(line);
                }
            }

            File.Move(filePath, Path.Combine(directoryLocation, fileName + ".CSV"));
        }

    }
}
