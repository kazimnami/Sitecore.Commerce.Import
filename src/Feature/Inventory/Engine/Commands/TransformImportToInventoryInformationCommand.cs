using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Commerce.Plugin.ManagedLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Inventory.Engine
{
    public class TransformImportToInventoryInformationCommand : CommerceCommand
    {
        private const int InventoryIdIndex = 0;
        private const int ProductIdIndex = 1;
        private const int QuantityIndex = 2;

        public TransformImportToInventoryInformationCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public Task<IEnumerable<InventoryInformation>> Process(CommerceContext commerceContext, IEnumerable<string[]> importRawLines)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var importPolicy = commerceContext.GetPolicy<ImportInventoryPolicy>();
                var importItems = new List<InventoryInformation>();
                foreach (var rawFields in importRawLines)
                {
                    var item = new InventoryInformation();
                    TransformCore(commerceContext, rawFields, item);
                    importItems.Add(item);
                }

                return Task.FromResult(importItems as IEnumerable<InventoryInformation>);
            }
        }

        private void TransformCore(CommerceContext commerceContext, string[] rawFields, InventoryInformation item)
        {
            var inventorySetName = rawFields[InventoryIdIndex];
            var productId = rawFields[ProductIdIndex];
            int.TryParse(rawFields[QuantityIndex], out int quantity);

            string str = inventorySetName + "-" + productId;
            //if (!string.IsNullOrEmpty(arg.VariationId)) str = str + "-" + arg.VariationId;
            item.Id = CommerceEntity.IdPrefix<InventoryInformation>() + str;
            item.FriendlyId = str;
            item.InventorySet = new EntityReference(inventorySetName.ToEntityId<InventorySet>(), "");
            item.SellableItem = new EntityReference(productId.ToEntityId<SellableItem>(), "");
            //item.VariationId = arg.VariationId;
            item.Quantity = quantity;
            var component = item.GetComponent<ListMembershipsComponent>();
            component.Memberships.Add(CommerceEntity.ListName<InventoryInformation>());

            var component1 = item.GetComponent<PreorderableComponent>();
            //component1.Preorderable = result1;
            //component1.PreorderAvailabilityDate = new DateTimeOffset?();
            //component1.PreorderLimit = new int?(0);

            var component2 = item.GetComponent<BackorderableComponent>();
            //component2.Backorderable = result3;
            //component2.BackorderAvailabilityDate = new DateTimeOffset?();
            //component2.BackorderLimit = new int?(0);
        }
    }
}
