using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Project.Import.CreateUploadFile
{
    public class Product
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public List<string> ImageUrlList { get; set; } = new List<string>();
        public List<string> ImageNameList { get; set; } = new List<string>();
        public List<string> CategoryIdList { get; set; } = new List<string>();
        public List<string> FoundOnPageList { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"PRODUCT: Id:{Id}";
        }

        public static Product AddUpdate(Dictionary<string, Product> list, Category category, string id, string displayName, HtmlAttribute url, string foundOnPage)
        {
            Product item = null;

            if (!list.ContainsKey(id))
            {
                item = new Product
                {
                    Id = id,
                    DisplayName = System.Net.WebUtility.HtmlDecode(displayName),
                    Url = url != null ? url.Value : "",
                    ImageUrlList = new List<string>(),
                    CategoryIdList = new List<string>(),
                };

                list.Add(id, item);

                Console.WriteLine("CREATING " + item.ToString());
            }
            else
            {
                item = list[id];

                Console.WriteLine("UPDATING " + item.ToString());
            }

            item.CategoryIdList.Add(category.Id);
            category.ProductIdList.Add(item.Id);
            item.FoundOnPageList.Add(foundOnPage);

            return item;
        }
    }
}
