using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Project.Import.CreateUploadFile.Sites
{
    public class ScraperCLRMTS : Scraper
    {
        public override void GetCategoryhierarchy(Config config, Dictionary<string, Category> categoryList)
        {
            var web = new HtmlWeb();
            var doc = web.Load(Config.Retrieve(config.Url));

            var nav = doc.DocumentNode.SelectSingleNode("//header/script");
            var navMenuScript = nav.InnerHtml.Remove(0, 94);
            navMenuScript = navMenuScript.Split(';')[0];
            dynamic menu = JsonConvert.DeserializeObject(navMenuScript);

            GetRootCategories(config, menu, categoryList);
        }

        private static void GetRootCategories(Config config, dynamic menu, Dictionary<string, Category> categoryList)
        {
            foreach (var categoryNode in menu.Children)
            {
                Console.WriteLine();
                Console.WriteLine($"***************************************************************************************");

                var category = Category.Add(config, categoryList, "", categoryNode.Name.Value, categoryNode.Url.Value);
                GetLevel1Categories(config, categoryNode, category, categoryList);
                if (config.DevMode) return; //only get one
            }
        }

        private static void GetLevel1Categories(Config config, dynamic startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            foreach (var categoryNode in startNode.Sections)
            {
                Category category = Category.Add(config, categoryList, parentCategory.Id, categoryNode.Name.Value, "");
                GetLevel2Categories(config, categoryNode, category, categoryList);
                if (config.DevMode) return; //only get one
            }
        }

        private static void GetLevel2Categories(Config config, dynamic startNode, Category parentCategory, Dictionary<string, Category> categoryList)
        {
            foreach (var node in startNode.Children)
            {
                Category.Add(config, categoryList, parentCategory.Id, node.Name.Value, node.Url.Value);
                if (config.DevMode) return; //only get one
            }
        }

        public override void GetCategoryToProductAssociation(Config config, Dictionary<string, Category> categoryList, Dictionary<string, Product> productList)
        {
            foreach (var category in categoryList.Values)
            {
                var url = category.Url;
                if (string.IsNullOrEmpty(url)) continue;
                url = (url.StartsWith("/")) ? Config.Retrieve(config.Url) + url : url;
                var urlWithItemsPerPage = url + "?ps=100";

                GetCategoryToProductAssociationByPage(config, productList, category, urlWithItemsPerPage);
            }

            //categoryList.Values.Where(c => c.ProductIdList.Count() == 0).ToList().ForEach(c => categoryList.Remove(c.Id));
        }

        private static void GetCategoryToProductAssociationByPage(Config config, Dictionary<string, Product> productList, Category category, string url)
        {
            Console.WriteLine();
            Console.WriteLine($"***************************************************************************************");
            Console.WriteLine("Retrieving product from page " + url);

            var web = new HtmlWeb();
            var doc = web.Load(url);

            var productNodeList = doc.DocumentNode.SelectNodes("//div[contains(@id, 'resultsContainer')]/div");

            if (productNodeList == null) return;

            foreach (var product in productNodeList)
            {
                var node = product.SelectSingleNode(".//div[@class='listing-title']/a");
                var productUrl = node.Attributes["href"];
                var productUri = new Uri(productUrl.Value);
                var productUriDirectLink = HttpUtility.ParseQueryString(productUri.Query).Get("amp;url");
                var imageNode = product.SelectSingleNode(".//img[@class='resultsTemplate_img']");
                var displayName = imageNode.Attributes["alt"].Value;
                var productId = product.SelectSingleNode(".//input[@class='rr-product-code']").Attributes["value"].Value;

                //if (productId == null) continue;
                Product.AddUpdate(productList, category, productId, displayName, productUriDirectLink, url);
            }

            var pagerNodeList = doc.DocumentNode.SelectNodes("//div[starts-with(@class, 'pagingTemplate_pageLinks')]/div");

            if (pagerNodeList == null) return;

            // will only move to the next page if the last page isn't the current page.
            for (int i = 0; i < pagerNodeList.Count() - 1; i++)
            {
                if ((pagerNodeList[i].Attributes["class"]?.Value.Equals("pagingTemplate_currentPage")).GetValueOrDefault())
                {
                    var nextPageUrl = url;
                    var pageIndex = nextPageUrl.IndexOf("&page=");
                    if (pageIndex > 0)
                    {
                        if (config.DevMode) return; //only get two pages
                        nextPageUrl = url.Substring(0, pageIndex);
                    }
                    nextPageUrl += "&page=" + pagerNodeList[i + 1].Attributes["data-facetid"].Value;
                    GetCategoryToProductAssociationByPage(config, productList, category, nextPageUrl);
                    break;
                }
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

                product.Price = doc.DocumentNode.SelectSingleNode("//h2[@class='now-price']")?.InnerHtml;
                if (product.Price == null) return;
                product.Price = product.Price.Replace("$", string.Empty);

                var imageNodeList = doc.DocumentNode.SelectNodes("//section[starts-with(@class, 'product-info')]/img");
                if (imageNodeList != null)
                {
                    foreach (var imageNode in imageNodeList)
                    {
                        product.ImageUrlList.Add(imageNode.Attributes["src"].Value);
                    }
                }

                product.Description = doc.DocumentNode.SelectSingleNode("//div[@itemprop='description']")?.InnerHtml;

                var detailsList = doc.DocumentNode.SelectNodes("//section[starts-with(@class, 'accordion tasting-note')]/div/div");
                if (detailsList != null)
                {
                    foreach (var detail in detailsList)
                    {
                        if (!string.IsNullOrEmpty(product.Manufacturer) && !string.IsNullOrEmpty(product.Brand)) break;

                        if ((detail.Element("h4")?.InnerText.Equals("Wine Region")).GetValueOrDefault())
                        {
                            product.Manufacturer = detail.Element("p").InnerText.ToString();
                        }
                        if ((detail.Element("h4")?.InnerText.Equals("Varieties")).GetValueOrDefault())
                        {
                            product.Brand = detail.Element("p").InnerText.ToString();
                        }
                    }
                }

                var breadCrumbList = doc.DocumentNode.SelectNodes("//ul[starts-with(@class, 'product breadcrumbs')]/li/a");
                if (breadCrumbList != null)
                {
                    product.TypeOfGood = breadCrumbList.ElementAt(1).InnerText;
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
