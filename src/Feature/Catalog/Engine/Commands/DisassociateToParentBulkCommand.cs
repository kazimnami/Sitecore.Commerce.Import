﻿using Foundation.Import.Engine;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class DisassociateToParentBulkCommand : CommerceCommand
    {
        public DisassociateToParentBulkCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<bool> Process(CommerceContext commerceContext, IEnumerable<CatalogItemParentAssociationModel> associationList)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                // TODO: This is not tested
                // TODO: draw insperation from Sitecore.Commerce.Plugin.Catalog.DoActionDisassociateBlock
                foreach (var association in associationList)
                {
                    // Need to clear message as any prior error will cause all transactions to abort.
                    commerceContext.ClearMessages();

                    await PerformTransaction(commerceContext, async () =>
                    {
                        var relationshipType = Command<GetRelationshipTypeCommand>().Process(commerceContext, association.ParentId, association.ItemId);
                        await Command<DeleteRelationshipCommand>().Process(commerceContext, association.ParentId, association.ItemId, relationshipType);
                    });
                }

                return true;
            }
        }
    }
}
