using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Project.Import.CreateUploadFile.Sites
{
    public class ScraperAMTF : Scraper
    {
        public override void GetCategoryhierarchy(Config config, Dictionary<string, Category> categoryList)
        {
            var web = new HtmlWeb();
            var doc = web.Load(Config.Retrieve(config.Url));

            var nav = doc.DocumentNode.SelectSingleNode("//div[@id='main_nav']");

            GetRootCategories(nav, categoryList);
            EnsureAllCategoryParentsExist(categoryList);
        }

        private void EnsureAllCategoryParentsExist(Dictionary<string, Category> categoryList)
        {
            foreach (var category in categoryList.Values)
            {
                if (string.IsNullOrEmpty(category.ParentCategoryId)) continue;
                if (!categoryList.TryGetValue(category.ParentCategoryId, out Category found))
                {
                    throw new Exception("Put a break point here and fix any category parents that don't exist.");
                }
            }
        }

        private static void GetRootCategories(HtmlNode startNode, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes("./ul/li");

            foreach (var categoryNode in categoryNodeList)
            {
                var node = categoryNode.SelectSingleNode("./a");

                if (node == null) continue;

                Console.WriteLine();
                Console.WriteLine($"***************************************************************************************");

                var category = Category.Add(categoryList, "", node.InnerHtml, node.Attributes["href"]);
                GetLevel1Categories(categoryNode, category, categoryList);
            }
        }

        private static void GetLevel1Categories(HtmlNode startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes("./div[starts-with(@class, 'large_dropdown')]/div/div");

            if (categoryNodeList == null) return;
            foreach (var categoryNode in categoryNodeList)
            {
                string categoryDisplayName = null;
                HtmlAttribute categoryUrl = null;

                var node = categoryNode.SelectSingleNode("./span[contains(@class, 'main_nav_heading')]/a");
                if (node != null)
                {
                    categoryUrl = node.Attributes["href"];
                    categoryDisplayName = node.InnerHtml.Replace("<br>", "");
                    if (categoryDisplayName.Contains("<"))
                        categoryDisplayName = "";
                }

                if (string.IsNullOrEmpty(categoryDisplayName)) continue;
                Category category = Category.Add(categoryList, parentCategory.Id, categoryDisplayName, categoryUrl);
                if (category == null) continue;


                GetLevel2Categories(categoryNode, category, categoryList);
            }
        }

        private static void GetLevel2Categories(HtmlNode startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes("./ul/li/a");

            if (categoryNodeList == null) return;
            foreach (var node in categoryNodeList)
            {
                Category.Add(categoryList, parentCategory.Id, node.InnerHtml, node.Attributes["href"]);
            }
        }

        public override void GetCategoryToProductAssociation(Config config, Dictionary<string, Category> categoryList, Dictionary<string, Product> productList)
        {
            foreach (var category in categoryList.Values)
            {
                var url = category.Url;
                url = (url.StartsWith("/")) ? Config.Retrieve(config.Url) + url : url;

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

            var productNodeList = doc.DocumentNode.SelectNodes("//ul[contains(@class, 'products-grid')]/li[contains(@class, 'item')]");

            if (productNodeList == null) return;

            foreach (var product in productNodeList)
            {
                var node = product.SelectSingleNode(".//div[@class='featured_product_image']/a");
                var productUrl = node.Attributes["href"];
                var imageNode = node.SelectSingleNode("./img");
                var displayName = imageNode.Attributes["alt"].Value;
                var productId = productUrl.Value.Substring(productUrl.Value.LastIndexOf('-') + 1, productUrl.Value.Length - productUrl.Value.LastIndexOf('-') - 2);

                try
                {
                    if (!int.TryParse(productId, out int n))
                    {
                        productId = imageNode.Attributes["src"].Value;
                        var firstIndex = productId.LastIndexOf('/');
                        var secondIndex = productId.LastIndexOf('_');
                        productId = productId.Substring(firstIndex + 1, secondIndex - firstIndex - 9);
                    }
                }
                catch { productId = null; }

                if (productId == null) continue;
                productId = productId.Replace("0002_002_", "");
                Product.AddUpdate(productList, category, productId, displayName, productUrl, url);
            }

            var pagerNodeList = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']/a[@title='Next']");

            if (pagerNodeList == null) return;

            var nextpageUrl = pagerNodeList.Attributes["href"].Value;
            GetCategoryToProductAssociationByPage(productList, category, nextpageUrl);
        }

        public override void GetProducts(Config config, Dictionary<string, Product> productList)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Getting further product details");

            foreach (var product in productList.Values)
            {
                Console.WriteLine("UPDATING " + product.ToString());

                var url = product.Url;
                url = (url.StartsWith("/")) ? Config.Retrieve(config.Url) + url : url;

                var web = new HtmlWeb();
                var doc = web.Load(url);

                product.Price = doc.DocumentNode.SelectSingleNode("//div[@class='price-box']//span[@class='price']")?.InnerHtml;
                if (product.Price == null) return;
                product.Price = product.Price.Remove(0, product.Price.LastIndexOf('>') + 1).Replace(",", "").TrimEnd();

                var imageNodeList = doc.DocumentNode.SelectNodes("//div[starts-with(@class, 'product-image-gallery')]/a/img");
                if (imageNodeList != null)
                {
                    foreach (var imageNode in imageNodeList)
                    {
                        product.ImageUrlList.Add(imageNode.Attributes["src"].Value);
                    }
                }
            }

            productList.Values.Where(p => p.Price == null).ToList().ForEach(c => productList.Remove(c.Id));
        }

        public override void GetImages(Config config, Dictionary<string, Product> productList)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Getting product images");

            var directoryInfo = Directory.CreateDirectory(config.DirectoryLocation);

            using (var webClient = new WebClient())
            {
                foreach (var product in productList.Values)
                {
                    for (int i = 0; i < product.ImageUrlList.Count(); i++)
                    {
                        var url = product.ImageUrlList.ElementAt(i);
                        url = (url.StartsWith("/")) ? Config.Retrieve(config.Url) + url : url;

                        var extensionStartIndex = url.LastIndexOf('.');
                        var fileName = $"{product.Id}_{i}{url.Substring(extensionStartIndex, url.Length - extensionStartIndex)}";

                        Console.WriteLine($"Downloading: '{url}' to '{fileName}'");
                        webClient.DownloadFile(url, Path.Combine(config.DirectoryLocation, fileName));
                        product.ImageNameList.Add(fileName);

                        // For the moment, only get 1 image per product
                        break;
                    }
                }
            }
        }

    }
}
