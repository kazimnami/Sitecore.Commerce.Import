using HtmlAgilityPack;
using Project.Import.CreateUploadFile.Sites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Project.Import.CreateUploadFile.Sites.BBQG
{
    public class ScraperBBQG : Scraper
    {
        public override void GetCategoryhierarchy(Config config, Dictionary<string, Category> categoryList)
        {
            var web = new HtmlWeb();
            var doc = web.Load(Config.Retrieve(config.Url));

            var nav = doc.DocumentNode.SelectSingleNode("//nav[@id='nav']");

            GetRootCategories(config, nav, categoryList);
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

            var productNodeList = doc.DocumentNode.SelectNodes("//ul[starts-with(@class, 'products-grid')]/li[@class='item']");

            if (productNodeList == null) return;

            foreach (var product in productNodeList)
            {

                var productId = product.SelectSingleNode(".//div[@class='trustpilot-widget']").Attributes["data-sku"].Value;
                var node = product.SelectSingleNode(".//h2[@class='product-name']/a");
                var productUrl = node.Attributes["href"];
                var displayName = node.Attributes["title"].Value;

                Product.AddUpdate(productList, category, productId, displayName, productUrl != null ? productUrl.Value : "", url);
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

        private static void GetRootCategories(Config config, HtmlNode startNode, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes(".//ol/ul/li[starts-with(@class, 'level0')]");

            foreach (var categoryNode in categoryNodeList)
            {
                var node = categoryNode.SelectSingleNode(".//a[contains(@class, 'level0')]");

                Console.WriteLine();
                Console.WriteLine($"***************************************************************************************");

                var category = Category.Add(config, categoryList, "", node.InnerHtml, node.Attributes["href"] != null ? node.Attributes["href"].Value : "");
                GetLevel1Categories(config, categoryNode, category, categoryList);
            }
        }

        private static void GetLevel1Categories(Config config, HtmlNode startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes(".//div[starts-with(@class, 'level1')]");

            if (categoryNodeList == null) return;
            foreach (var categoryNode in categoryNodeList)
            {
                var node = categoryNode.SelectSingleNode(".//a[contains(@class, 'level1')]");

                if (node == null) continue;

                var category = Category.Add(config, categoryList, parentCategory.Id, node.InnerHtml, node.Attributes["href"] != null ? node.Attributes["href"].Value : "");
                GetLevel2Categories(config, categoryNode, category, categoryList);
            }
        }

        private static void GetLevel2Categories(Config config, HtmlNode startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            var categoryNodeList = startNode.SelectNodes(".//li[starts-with(@class, 'level2')]");

            if (categoryNodeList == null) return;
            foreach (var categoryNode in categoryNodeList)
            {
                var node = categoryNode.SelectSingleNode(".//a");

                Category.Add(config, categoryList, parentCategory.Id, node.InnerHtml, node.Attributes["href"] != null ? node.Attributes["href"].Value : "");
            }
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

                product.Price = doc.DocumentNode.SelectSingleNode("//span[@class='price']")?.InnerHtml;
                if (product.Price == null) return;
                product.Price = product.Price.Remove(0, product.Price.LastIndexOf('>') + 1).Replace(",", "").TrimEnd();

                var imageNodeList = doc.DocumentNode.SelectNodes("//div[starts-with(@class, 'main-image-set')]/div/img");
                foreach (var imageNode in imageNodeList)
                {
                    product.ImageUrlList.Add(imageNode.Attributes["src"].Value);
                }
            }

            productList.Values.Where(p => p.Price == null).ToList().ForEach(c => productList.Remove(c.Id));
        }

        public override void GetImages(Config config, Dictionary<string, Product> productList)
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
                        url = (url.StartsWith("/")) ? Config.Retrieve(config.Url) + url : url;

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
    }
}
