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
    }
}
