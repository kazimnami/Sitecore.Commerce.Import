using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class CopyImportToSellableItemsCommand : CommerceCommand
    {
        public CopyImportToSellableItemsCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public void Process(CommerceContext commerceContext, IEnumerable<SellableItem> allImportItems, IEnumerable<SellableItem> changedItems)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                foreach (var item in changedItems)
                {
                    var itemNewData = allImportItems.FirstOrDefault(i => i.ProductId.Equals(item.ProductId));

                    if (itemNewData == null) throw new Exception($"Error, can't find product Id {item.ProductId} in import list. This should never happen.");

                    CopyCore(itemNewData, item);
                    CopyListPrice(itemNewData, item);
                    CopyProductExtension(itemNewData, item);
                    CopyCatalog(itemNewData, item);
                    CopyCategory(itemNewData, item);
                    CopyImages(itemNewData, item);
                }
            }
        }

        private void CopyCore(SellableItem itemNewData, SellableItem item)
        {
            // These fields are not included
            // item.ProductId
            // item.Id
            // item.FriendlyId 
            // item.Name 
            // item.SitecoreId
            item.DisplayName = itemNewData.DisplayName;
            item.Description = itemNewData.Description;
            item.Brand = itemNewData.Brand;
            item.Manufacturer = itemNewData.Manufacturer;
            item.TypeOfGood = itemNewData.TypeOfGood;
            item.Tags = itemNewData.Tags;
        }

        private void CopyListPrice(SellableItem itemNewData, SellableItem item)
        {
            item.RemovePolicy(typeof(ListPricingPolicy));
            item.Policies.Add(itemNewData.GetPolicy<ListPricingPolicy>());
        }

        private void CopyProductExtension(SellableItem itemNewData, SellableItem item)
        {
            item.RemoveComponent(typeof(ProductExtensionComponent));
            item.Components.Add(itemNewData.GetComponent<ProductExtensionComponent>());
        }

        private void CopyCatalog(SellableItem itemNewData, SellableItem item)
        {
            item.ParentCatalogList = itemNewData.ParentCatalogList;
        }

        private void CopyCategory(SellableItem itemNewData, SellableItem item)
        {

            item.ParentCategoryList = itemNewData.ParentCategoryList;
        }

        private void CopyImages(SellableItem itemNewData, SellableItem item)
        {
            item.RemoveComponent(typeof(ImagesComponent));
            item.Components.Add(itemNewData.GetComponent<ImagesComponent>());
        }
    }
}
