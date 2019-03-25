using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Inventory.Engine
{
    public class AssociateInventoryToSellableItemCommand : CommerceCommand
    {
        public AssociateInventoryToSellableItemCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task Process(CommerceContext commerceContext, List<InventoryInformation> inventoryInformationItems)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                foreach (var item in inventoryInformationItems)
                {
                    var versions = await this.Pipeline<IFindEntityVersionsPipeline>().Run(new FindEntityArgument(typeof(SellableItem), item.SellableItem.EntityTarget, false), commerceContext.PipelineContextOptions);
                    foreach (SellableItem instance in versions.Cast<SellableItem>())
                    {
                        var associations = instance.GetComponent<InventoryComponent>().InventoryAssociations;
                        if (!associations.Any(a => a.InventoryInformation.EntityTarget.Equals(item.Id) && a.InventorySet.EntityTarget.Equals(item.InventorySet.EntityTarget)))
                        {
                            associations.Add(new InventoryAssociation()
                            {
                                InventoryInformation = new EntityReference(item.Id, ""),
                                InventorySet = new EntityReference(item.InventorySet.EntityTarget, "")
                            });
                            await this.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(instance), commerceContext.PipelineContextOptions);
                        }

                        //ItemVariationComponent variation = instance.GetVariation(arg.VariationId);
                        //if (variation != null || !versions.Cast<SellableItem>().Any<SellableItem>((Func<SellableItem, bool>)(version => version.GetVariation(arg.VariationId) != null)))
                        //{
                        //    (variation != null ? variation.GetComponent<InventoryComponent>() : instance.GetComponent<InventoryComponent>()).InventoryAssociations.Add(new InventoryAssociation()
                        //    {
                        //        InventoryInformation = new EntityReference(inventoryInformation.Id, ""),
                        //        InventorySet = new EntityReference(arg.InventorySetId, "")
                        //    });
                        //    configuredTaskAwaitable = inventorySetBlock._persistEntityPipeline.Run(new PersistEntityArgument((CommerceEntity)instance), context).ConfigureAwait(false);
                        //    PersistEntityArgument persistEntityArgument2 = await configuredTaskAwaitable;
                        //}
                    }
                }
            }
        }
    }
}
