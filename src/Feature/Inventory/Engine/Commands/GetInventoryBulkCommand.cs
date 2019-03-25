using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Inventory.Engine
{
    public class GetInventoryBulkCommand : CommerceCommand
    {
        public GetInventoryBulkCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<IEnumerable<InventoryInformation>> Process(CommerceContext commerceContext, IEnumerable<InventoryInformation> items)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var returnedItems = new List<InventoryInformation>();
                foreach (var item in items)
                {
                    var findEntityArgument = new FindEntityArgument(typeof(InventoryInformation), item.Id, false);

                    // No bulk API exists in core platform, under the hood it's reading one at a time.
                    // I've layed out the import in this manner so you can test how long each piece takes
                    // An option you have is to do a select directly against the DB, for all items in this batch 
                    // to see if you need to make this call.
                    if (await Pipeline<IFindEntityPipeline>().Run(findEntityArgument, commerceContext.PipelineContextOptions) is InventoryInformation commerceEntity)
                    {
                        returnedItems.Add(commerceEntity);
                    }
                }

                return returnedItems;
            }
        }
    }
}
