using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Feature.Catalog.Engine
{
    public class ImportSellableItemComparer : IEqualityComparer<SellableItem>
    {
        private readonly SellableItemComparerConfiguration Configuration;
        private readonly TagComparer SellableItemTagComparer;
        private readonly MoneyComparer SellableItemMoneyComparer;
        private readonly ProductExtensionComponentComparer SellableItemProductExtensionComponentComparer;

        public ImportSellableItemComparer(SellableItemComparerConfiguration configuration)
        {
            Configuration = configuration;
            SellableItemTagComparer = new TagComparer();
            SellableItemMoneyComparer = new MoneyComparer();
            SellableItemProductExtensionComponentComparer = new ProductExtensionComponentComparer();
        }

        public bool Equals(SellableItem x, SellableItem y)
        {
            if (x == null || y == null) return false;

            switch (Configuration)
            {
                case SellableItemComparerConfiguration.ByProductId:
                    return x.ProductId == y.ProductId;

                case SellableItemComparerConfiguration.ByImportData:
                    return SellableItemCoreMemberEquality(x, y)
                        && ListPriceEquality(x.GetPolicy<ListPricingPolicy>(), y.GetPolicy<ListPricingPolicy>())
                        && ImagesEquality(x.GetComponent<ImagesComponent>(), y.GetComponent<ImagesComponent>())
                        && ProductExtensionComponentEquality(x.GetComponent<ProductExtensionComponent>(), y.GetComponent<ProductExtensionComponent>())
                        // TODO: Variants
                        ;

                default:
                    throw new ArgumentException($"Comparer configuration cannot be handled");
            }
        }

        private bool SellableItemCoreMemberEquality(SellableItem x, SellableItem y)
        {
            return x.ProductId == y.ProductId
                && x.Name == y.Name
                && x.DisplayName == y.DisplayName
                && x.Description == y.Description
                && x.Brand == y.Brand
                && x.Manufacturer == y.Manufacturer
                && x.TypeOfGood == y.TypeOfGood
                && TagsEquality(x.Tags, y.Tags)
                && StringListEquality(x.ParentCatalogList, y.ParentCatalogList)
                && StringListEquality(x.ParentCategoryList, y.ParentCategoryList);
        }

        private bool TagsEquality(IList<Tag> x, IList<Tag> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase).SequenceEqual(y.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase), SellableItemTagComparer);
        }

        private bool StringListEquality(string x, string y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Split('|').OrderBy(i => i, StringComparer.OrdinalIgnoreCase).SequenceEqual(y.Split('|').OrderBy(i => i, StringComparer.OrdinalIgnoreCase));
        }

        private bool ListPriceEquality(ListPricingPolicy x, ListPricingPolicy y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Prices == null && y.Prices == null) return true;
            if (x.Prices == null || y.Prices == null) return false;
            return x.Prices.OrderBy(p => p.CurrencyCode, StringComparer.OrdinalIgnoreCase).SequenceEqual(y.Prices.OrderBy(p => p.CurrencyCode, StringComparer.OrdinalIgnoreCase), SellableItemMoneyComparer);
        }

        private bool ImagesEquality(ImagesComponent x, ImagesComponent y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Images == null && y.Images == null) return true;
            if (x.Images == null || y.Images == null) return false;
            return x.Images.OrderBy(i => i, StringComparer.OrdinalIgnoreCase).SequenceEqual(y.Images.OrderBy(i => i, StringComparer.OrdinalIgnoreCase));
        }

        private bool ProductExtensionComponentEquality(ProductExtensionComponent x, ProductExtensionComponent y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return SellableItemProductExtensionComponentComparer.Equals(x, y);
        }

        public int GetHashCode(SellableItem obj)
        {
            if (obj == null) return base.GetHashCode();

            // https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked
            {
                int hash = 17;

                switch (Configuration)
                {
                    case SellableItemComparerConfiguration.ByProductId:
                        if (obj.ProductId != null) hash = hash * 23 + obj.ProductId.GetHashCode();
                        break;

                    case SellableItemComparerConfiguration.ByImportData:
                        if (obj.ProductId != null) hash = hash * 23 + obj.ProductId.GetHashCode();
                        if (obj.Name != null) hash = hash * 23 + obj.Name.GetHashCode();
                        if (obj.DisplayName != null) hash = hash * 23 + obj.DisplayName.GetHashCode();
                        if (obj.Description != null) hash = hash * 23 + obj.Description.GetHashCode();
                        if (obj.Brand != null) hash = hash * 23 + obj.Brand.GetHashCode();
                        if (obj.Manufacturer != null) hash = hash * 23 + obj.Manufacturer.GetHashCode();
                        if (obj.TypeOfGood != null) hash = hash * 23 + obj.TypeOfGood.GetHashCode();
                        if (obj.Tags != null) obj.Tags.ForEach(tag => hash = hash * 23 + SellableItemTagComparer.GetHashCode(tag));
                        if (obj.ParentCatalogList != null) obj.ParentCatalogList.Split('|').OrderBy(t => t, StringComparer.OrdinalIgnoreCase).ForEach(t => hash = hash * 23 + t.GetHashCode());
                        if (obj.ParentCategoryList != null) obj.ParentCategoryList.Split('|').OrderBy(t => t, StringComparer.OrdinalIgnoreCase).ForEach(t => hash = hash * 23 + t.GetHashCode());
                        obj.GetPolicy<ListPricingPolicy>().Prices.ForEach(price => hash = hash * 23 + SellableItemMoneyComparer.GetHashCode(price)); // View tests - Null exception is not possible
                        obj.GetComponent<ImagesComponent>().Images?.ForEach(image => hash = hash * 23 + image.GetHashCode());
                        hash = hash * 23 + SellableItemProductExtensionComponentComparer.GetHashCode(obj.GetComponent<ProductExtensionComponent>());
                        // TODO: Variant                        
                        break;

                    default:
                        throw new ArgumentException($"Comparer configuration cannot be handled");
                }

                return hash;
            }
        }
    }

    public enum SellableItemComparerConfiguration
    {
        ByProductId,
        ByImportData
    }
}
