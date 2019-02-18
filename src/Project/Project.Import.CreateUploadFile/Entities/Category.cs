using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Project.Import.CreateUploadFile.Entities
{
    public class Category
    {
        public string Id { get; set; }
        public string ParentCategoryId { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public List<string> ProductIdList { get; set; }

        public override string ToString()
        {
            return $"CATEGORY: Id:{Id}, DisplayName:{DisplayName}, URL:{Url}";
        }

        public static Category Add(Dictionary<string, Category> categoryList, string parentCategoryId, string level, string displayName, HtmlAttribute url)
        {
            var item = new Category
            {
                Id = (level + "_" + displayName).ToLower().Replace(" ", "_"),
                ParentCategoryId = parentCategoryId,
                DisplayName = displayName,
                Url = url != null ? url.Value : "",
                ProductIdList = new List<string>(),
            };

            Console.WriteLine(item.ToString());

            categoryList.Add(item.Id, item);
            return item;
        }
    }
}
