using System;
using System.Text;

namespace Project.Import.CreateUploadFile.Sites
{
    public abstract class Config
    {
        public string Url { get; set; }
        public string CatalogName { get; set; }
        public string DirectoryLocation { get; set; } = @"c:\Import\Images";
        public bool UseParentCategoryNameInChildren { get; set; } = false;
        public bool DevMode { get; set; } = false;

        public static string Set(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string Retrieve(string input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }
    }
}
