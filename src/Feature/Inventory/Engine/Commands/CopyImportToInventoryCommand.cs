using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Commerce.Plugin.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Inventory.Engine
{
    public class CopyImportToInventoryCommand : CommerceCommand
    {
        public CopyImportToInventoryCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task Process(CommerceContext commerceContext, IEnumerable<InventoryInformation> allImportItems, IEnumerable<InventoryInformation> changedItems)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                if (!changedItems.Any()) return;

                foreach (var item in changedItems)
                {
                    var itemNewData = allImportItems.FirstOrDefault(i => i.Id.Equals(item.Id));

                    if (itemNewData == null) throw new Exception($"Error, can't find item Id {item.Id} in import list. This should never happen.");

                    CopyCore(itemNewData, item);
                }
            }
        }

        private void CopyCore(InventoryInformation itemNewData, InventoryInformation item)
        {
            // These fields are not included
            // item.ProductId
            // item.Id
            // item.FriendlyId 
            // item.Name 
            // item.SitecoreId
            item.Quantity = itemNewData.Quantity;
            // item.Description = itemNewData.Description; // Not being imported at the moment so expect it will be managed manually.
        }
    }
}
