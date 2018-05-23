using Sitecore.Commerce.Core;

namespace Feature.Catalog.Engine
{
    public class KnownProductExtensionActionsPolicy : Policy
    {
        public string ProductExtensionEdit { get; set; } = nameof(ProductExtensionEdit);
    }
}