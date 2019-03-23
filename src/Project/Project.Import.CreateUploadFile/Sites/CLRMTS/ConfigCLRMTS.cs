namespace Project.Import.CreateUploadFile.Sites
{
    public class ConfigCLRMTS : Config
    {
        public ConfigCLRMTS()
        {
            Url = "aHR0cHM6Ly93d3cuY2VsbGFybWFzdGVycy5jb20uYXUv";
            CatalogName = "CLRMTS"; // "Habitat_Master";
            DirectoryLocation = @"c:\Import\Images\" + CatalogName;
            UseParentCategoryNameInChildren = true;
            DevMode = false;
        }
    }
}
