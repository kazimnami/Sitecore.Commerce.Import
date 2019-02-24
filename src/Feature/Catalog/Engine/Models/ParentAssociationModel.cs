using Sitecore.Commerce.Plugin.Catalog;
using System;

namespace Feature.Catalog.Engine
{
    public class ParentAssociationModel
    {
        public string CatalogId { get; }
        public string ParentId { get; }

        private CatalogItemBase parent;

        public string ParentSitecoreId { get { return parent != null ? parent.SitecoreId : throw new Exception("Error, parent property is null"); } }

        public ParentAssociationModel(string catalogId, CatalogItemBase parent)
        {
            this.CatalogId = catalogId;
            this.ParentId = parent.Id;
            this.parent = parent;
        }

        public ParentAssociationModel(string catalogId, string parentId)
        {
            this.CatalogId = catalogId;
            this.ParentId = parentId;
        }
    }
}
