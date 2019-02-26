using Sitecore.Commerce.Plugin.Catalog;

namespace Feature.Catalog.Engine
{
    public class ParentAssociationModel
    {
        public string ItemId { get; set; }
        public string CatalogId { get; set; }
        public string ParentId { get; set; }
        public string ParentSitecoreId { get; set; }

        public ParentAssociationModel() { }

        public ParentAssociationModel(string itemId, string catalogId, CatalogItemBase parent)
        {
            ItemId = itemId;
            CatalogId = catalogId;
            ParentId = parent.Id;
            ParentSitecoreId = parent.SitecoreId;
        }

        public ParentAssociationModel(string itemId, string catalogId, string parentId)
        {
            ItemId = itemId;
            CatalogId = catalogId;
            ParentId = parentId;
        }
    }
}
