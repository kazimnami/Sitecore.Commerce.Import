using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class AssociateToParentBulkCommand : CommerceCommand
    {
        public AssociateToParentBulkCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<bool> Process(CommerceContext commerceContext, IEnumerable<ParentAssociationModel> associationList)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                foreach (var association in associationList)
                {
                    commerceContext.ClearModels();
                    var relationshipType = Command<GetRelationshipTypeCommand>().Process(commerceContext, association.ParentId, association.ItemId);
                    await Command<CreateRelationshipCommand>().Process(commerceContext, association.ParentId, association.ItemId, relationshipType);
                }

                return true;
            }
        }
    }
}
