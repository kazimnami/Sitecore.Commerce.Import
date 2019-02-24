namespace Feature.Catalog.Engine
{
    public class CatalogAssociationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SitecoreId { get; set; }

        public CatalogAssociationModel() { }

        public CatalogAssociationModel(Sitecore.Commerce.Plugin.Catalog.Catalog catalog)
        {
            Id = catalog.Id;
            Name = catalog.Name;
            SitecoreId = catalog.SitecoreId;
        }
    }
}
