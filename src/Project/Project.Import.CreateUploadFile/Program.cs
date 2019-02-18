using HtmlAgilityPack;
using Project.Import.CreateUploadFile.Entities;
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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var categoryList = new Dictionary<string, Category>();
            var productList = new Dictionary<string, Product>();

            GetCategoryhierarchy(categoryList);
            GetCategoryToProductAssociation(categoryList, productList);
            //GetCategories();
            GetProducts(productList);
            GetImages(productList);
            CreateFile(productList);
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
                return;
            }
        }

        private static void GetCategoryToProductAssociationByPage(Dictionary<string, Product> productList, Category category, string url)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Retrieving product from page " + url);

            var web = new HtmlWeb();
            var doc = web.Load(url);

            var productNodeList = doc.DocumentNode.SelectNodes("//ul[starts-with(@class, 'products-grid')]/li[@class='item']");

            foreach (var product in productNodeList)
            {

                var productId = product.SelectSingleNode(".//div[@class='trustpilot-widget']").Attributes["data-sku"].Value;
                var node = product.SelectSingleNode(".//h2[@class='product-name']/a");
                var productUrl = node.Attributes["href"];
                var displayName = node.Attributes["title"].Value;

                Product.AddUpdate(productList, category, productId, displayName, productUrl);
                return;
            }

            var pagerNodeList = doc.DocumentNode.SelectNodes("//div[@class='pages']/ol/li");
            return;

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

        private static void GetRootCategories(HtmlNode categoryNode, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = categoryNode.SelectNodes(".//ol/ul/li[starts-with(@class, 'level0')]");

            foreach (var category in categoryNodeList)
            {
                var node = category.SelectSingleNode(".//a[contains(@class, 'level0')]");

                Console.WriteLine();
                Console.WriteLine($"***************************************************************************************");

                Category.Add(categoryList, "", "level0", node.InnerHtml, node.Attributes["href"]);
                return;
                GetLevel1Categories(category, categoryList);
            }
        }

        private static void GetLevel1Categories(HtmlNode categoryNode, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = categoryNode.SelectNodes(".//div[starts-with(@class, 'level1')]");

            if (categoryNodeList == null) return;
            foreach (var category in categoryNodeList)
            {
                var node = category.SelectSingleNode(".//a[contains(@class, 'level1')]");

                if (node == null) continue;

                Category.Add(categoryList, "", "level1", node.InnerHtml, node.Attributes["href"]);

                GetLevel2Categories(category, categoryList);
            }
        }

        private static void GetLevel2Categories(HtmlNode categoryNode, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = categoryNode.SelectNodes(".//li[starts-with(@class, 'level2')]");

            if (categoryNodeList == null) return;
            foreach (var category in categoryNodeList)
            {
                var node = category.SelectSingleNode(".//a");

                Category.Add(categoryList, "", "level2", node.InnerHtml, node.Attributes["href"]);
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

                product.Price = doc.DocumentNode.SelectSingleNode("//span[@class='price']").InnerHtml;
                product.Price = product.Price.Remove(0, product.Price.LastIndexOf('>') + 1).Replace(",", "").TrimEnd();

                var imageNodeList = doc.DocumentNode.SelectNodes("//div[starts-with(@class, 'main-image-set')]/div/img");
                foreach (var imageNode in imageNodeList)
                {
                    product.ImageUrlList.Add(imageNode.Attributes["src"].Value);
                }
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
            var filePath = Path.Combine(directoryLocation, fileName, ".csv");
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
                    "CategoryName", // 13
                    "Style", // 14
                    "FuelType", // 15
                    "NaturalGasConversionAvailable", // 15
                    "DimensionsHeightHoodOpen", // 16
                    "DimensionsHeightHoodClosed", // 17
                    "DimensionsWidth", // 18
                    "DimensionsDepth", // 19
                };

                file.WriteLine(string.Join(',', headerList));

                var line = new StringBuilder();
                foreach (var product in productList.Values)
                {
                    line.Clear();
                    line.Append(product.Id + ","); //"ProductId", // 0
                    line.Append(product.DisplayName + ","); //"ProductName", // 1
                    line.Append(product.DisplayName + ",");//"DisplayName", // 2
                    line.Append(product.Description + ","); //"Description", // 3
                    line.Append(",");//"Brand", // 4
                    line.Append(",");//"Manufacturer", // 5
                    line.Append(",");//"TypeOfGood", // 6
                    line.Append(",");//"Tags", // 7
                    line.Append("USD-" + product.Price + ",");//"ListPrice", // 8
                    line.Append(string.Join('|', product.ImageNameList) + ",");//"Images", // 9
                    line.Append("BBQG,");//"CatalogName", // 10
                    line.Append(string.Join('|', product.CategoryIdList) + ",");//"CategoryName", // 13
                    line.Append(",");//"Style", // 14
                    line.Append(",");//"FuelType", // 15
                    line.Append(",");//"NaturalGasConversionAvailable", // 15
                    line.Append(",");//"DimensionsHeightHoodOpen", // 16
                    line.Append(",");//"DimensionsHeightHoodClosed", // 17
                    line.Append(",");//"DimensionsWidth", // 18
                    //line.Append();//"DimensionsDepth", // 19

                    file.WriteLine(line);
                }
            }

            File.Move(filePath, Path.Combine(directoryLocation, fileName, ".CSV"));
        }
    }
}
