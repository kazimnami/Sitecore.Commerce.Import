using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Feature.Inventory.Engine
{
    public class ImportInventoryComparer : IEqualityComparer<InventoryInformation>
    {
        private readonly InventoryComparerConfiguration Configuration;

        public ImportInventoryComparer(InventoryComparerConfiguration configuration)
        {
            Configuration = configuration;
        }

        public bool Equals(InventoryInformation x, InventoryInformation y)
        {
            if (x == null || y == null) return false;

            switch (Configuration)
            {
                case InventoryComparerConfiguration.ById:
                    return x.Id == y.Id;

                case InventoryComparerConfiguration.ByData:
                    return x.Id == y.Id
                        && x.Name == y.Name
                        && x.Quantity == y.Quantity;

                default:
                    throw new ArgumentException($"Comparer configuration cannot be handled");
            }
        }

        public int GetHashCode(InventoryInformation obj)
        {
            if (obj == null) return base.GetHashCode();

            // https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked
            {
                int hash = 17;

                switch (Configuration)
                {
                    case InventoryComparerConfiguration.ById:
                        if (obj.Id != null) hash = hash * 23 + obj.Id.GetHashCode();
                        break;

                    case InventoryComparerConfiguration.ByData:
                        if (obj.Id != null) hash = hash * 23 + obj.Id.GetHashCode();
                        if (obj.Name != null) hash = hash * 23 + obj.Name.GetHashCode();
                        hash = hash * 23 + obj.Quantity.GetHashCode();
                        break;

                    default:
                        throw new ArgumentException($"Comparer configuration cannot be handled");
                }

                return hash;
            }
        }
    }

    public enum InventoryComparerConfiguration
    {
        ById,
        ByData
    }
}
