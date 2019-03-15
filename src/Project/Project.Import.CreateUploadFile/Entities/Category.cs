using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Project.Import.CreateUploadFile
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
            return $"CATEGORY: Id:{Id}, DisplayName:{DisplayName}";
        }

        public static Category Add(Dictionary<string, Category> categoryList, string parentCategoryId, string displayName, HtmlAttribute url)
        {
            displayName = WebUtility.HtmlDecode(displayName);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var id = textInfo.ToTitleCase(RemoveSpecialChars(displayName.ToLower())).Replace(" ", "");

            var item = new Category
            {
                //Id = string.IsNullOrEmpty(parentCategoryId) ? id : $"{parentCategoryId}_{id}",
                Id = id,
                ParentCategoryId = parentCategoryId,
                DisplayName = displayName,
                Url = url != null ? url.Value : "",
                ProductIdList = new List<string>(),
            };

            Console.WriteLine(item.ToString());

            try
            {
                categoryList.Add(item.Id, item);
            }
            catch (ArgumentException) {}

            return item;
        }

        public static string RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[^0-9a-z ]", string.Empty, RegexOptions.Compiled);
        }
    }
}
