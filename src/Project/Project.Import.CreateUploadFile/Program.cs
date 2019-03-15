using HtmlAgilityPack;
using Project.Import.CreateUploadFile.Sites;
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
            Config config = new ConfigAMTF();
            Scraper scraper = new ScraperAMTF();

            try
            {
                var categoryList = new Dictionary<string, Category>();
                var productList = new Dictionary<string, Product>();

                scraper.GetCategoryhierarchy(config, categoryList);
                scraper.GetCategoryToProductAssociation(config, categoryList, productList);
                //scraper.GetCategories();
                scraper.GetProducts(config, productList);
                scraper.CleanProductCategories(productList);
                scraper.GetImages(config, productList);

                CreateFile(config, productList);
                CreateFile(config, categoryList);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.ReadLine();
            }
        }

        private static void CreateFile(Config config, Dictionary<string, Product> productList)
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
                    line.Append(config.CatalogName + product.Id + ","); //"ProductId", // 0
                    line.Append(product.DisplayName?.StringToCSVCell() + ","); //"ProductName", // 1
                    line.Append(product.DisplayName?.StringToCSVCell() + ",");//"DisplayName", // 2
                    line.Append(product.Description?.StringToCSVCell() + ","); //"Description", // 3
                    line.Append(",");//"Brand", // 4
                    line.Append(",");//"Manufacturer", // 5
                    line.Append(",");//"TypeOfGood", // 6
                    line.Append(",");//"Tags", // 7
                    line.Append("USD-" + product.Price + ",");//"ListPrice", // 8
                    line.Append(string.Join('|', product.ImageNameList) + ",");//"Images", // 9
                    line.Append(config.CatalogName + ",");//"CatalogName", // 10
                    product.CategoryIdList.ForEach(c => line.Append(string.Join("^^", $"{config.CatalogName}^-{c}"))); //"CategoryName", // 11
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

        private static void CreateFile(Config config, Dictionary<string, Category> categoryList)
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
                    line.Append(config.CatalogName + ","); //"CatalogName", // 0
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
