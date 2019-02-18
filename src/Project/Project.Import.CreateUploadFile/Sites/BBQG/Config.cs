using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Import.CreateUploadFile.Sites.BBQG
{
    public static class Config
    {
        public static readonly string Url = "aHR0cHM6Ly93d3cuYmFyYmVxdWVzZ2Fsb3JlLmNvbS5hdS8=";

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
