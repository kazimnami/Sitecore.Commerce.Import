using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Import.CreateUploadFile.Sites
{
    public class ConfigAMTF : Config
    {
        public ConfigAMTF()
        {
            Url = "aHR0cHM6Ly93d3cuYW1hcnRmdXJuaXR1cmUuY29tLmF1Lw==";
            CatalogName = "AMTF"; // "Habitat_Master";
            DirectoryLocation = @"c:\Import\Images" + CatalogName;
        }
    }
}
