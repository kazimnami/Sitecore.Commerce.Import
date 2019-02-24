using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class DisassociateSellableItemToParentBulkCommand : CommerceCommand
    {
        public DisassociateSellableItemToParentBulkCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<bool> Process(CommerceContext commerceContext, IEnumerable<SellableItem> items)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                // TODO: draw insperation from Sitecore.Commerce.Plugin.Catalog.DoActionDisassociateBlock

                return true;
            }
        }
    }
}
