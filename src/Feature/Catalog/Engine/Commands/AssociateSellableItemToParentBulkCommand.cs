using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class AssociateSellableItemToParentBulkCommand : CommerceCommand
    {
        public AssociateSellableItemToParentBulkCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<bool> Process(CommerceContext commerceContext, IEnumerable<SellableItem> items)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                foreach (var item in items)
                {
                    await PerformTransaction(commerceContext, async () =>
                    {
                        var tran = item.GetPolicy<TransientImportSellableItemDataPolicy>();
                        foreach (var ass in tran.ParentAssociationsToCreateList)
                        {
                            await Command<AssociateSellableItemToParentCommand>().Process(commerceContext, ass.CatalogId, ass.ParentId, item.Id);
                        }
                    });
                }

                return true;
            }
        }
    }
}
