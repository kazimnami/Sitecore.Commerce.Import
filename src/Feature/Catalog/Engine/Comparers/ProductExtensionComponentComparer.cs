using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Feature.Catalog.Engine
{
    public class ProductExtensionComponentComparer : IEqualityComparer<ProductExtensionComponent>
    {
        public bool Equals(ProductExtensionComponent x, ProductExtensionComponent y)
        {
            return ProductExtensionComponent.MemberEquality(x, y);
        }

        public int GetHashCode(ProductExtensionComponent obj)
        {
            return ProductExtensionComponent.GetHashCodeMembers(obj);
        }
    }
}
