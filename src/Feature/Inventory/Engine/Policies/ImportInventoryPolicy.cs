using Foundation.Import.Engine;

namespace Feature.Inventory.Engine
{
    public class ImportInventoryPolicy : ImportPolicy
    {
        public ImportInventoryPolicy()
        {
            this.FilePrefix = "InventoryImport";
        }
    }
}
