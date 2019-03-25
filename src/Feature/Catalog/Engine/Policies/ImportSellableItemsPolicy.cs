using Foundation.Import.Engine;

namespace Feature.Catalog.Engine
{
    public class ImportSellableItemsPolicy : ImportPolicy
    {
        public ImportSellableItemsPolicy()
        {
            this.FilePrefix = "ProductImport";
        }
    }
}
