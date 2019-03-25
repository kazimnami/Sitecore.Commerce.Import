using Foundation.Import.Engine;

namespace Feature.Catalog.Engine
{
    public class ImportCategoriesPolicy : ImportPolicy
    {
        public ImportCategoriesPolicy()
        {
            this.FilePrefix = "CategoryImport";
        }
    }
}
