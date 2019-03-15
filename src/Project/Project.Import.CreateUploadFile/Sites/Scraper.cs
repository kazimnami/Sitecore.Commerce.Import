using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Import.CreateUploadFile.Sites
{
    public abstract class Scraper
    {
        public abstract void GetCategoryhierarchy(Config config, Dictionary<string, Category> categoryList);
        public abstract void GetCategoryToProductAssociation(Config config, Dictionary<string, Category> categoryList, Dictionary<string, Product> productList);
        public abstract void GetProducts(Config config, Dictionary<string, Product> productList);
        public abstract void GetImages(Config config, Dictionary<string, Product> productList);

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
    }
}
