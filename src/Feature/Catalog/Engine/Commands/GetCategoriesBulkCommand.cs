using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class GetCategoriesBulkCommand : CommerceCommand
    {
        public GetCategoriesBulkCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<IEnumerable<Category>> Process(CommerceContext commerceContext, IEnumerable<Category> items)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var returnedItems = new List<Category>();
                foreach (var item in items)
                {
                    var findEntityArgument = new FindEntityArgument(typeof(Category), item.Id, false);

                    // No bulk API exists in core platform, under the hood it's reading one at a time.
                    // I've layed out the import in this manner so you can test how long each piece takes
                    // An option you have is to do a select directly against the DB, for all items in this batch 
                    // to see if you need to make this call.
                    if (await Pipeline<IFindEntityPipeline>().Run(findEntityArgument, commerceContext.PipelineContextOptions) is Category commerceEntity)
                    {
                        returnedItems.Add(commerceEntity);
                    }
                }

                return returnedItems;
            }
        }
    }
}
