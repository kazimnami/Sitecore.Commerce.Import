using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class GetRelationshipTypeCommand : CommerceCommand
    {
        public GetRelationshipTypeCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public string Process(CommerceContext commerceContext, string parentId, string itemId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                if (parentId.StartsWith(CommerceEntity.IdPrefix<Sitecore.Commerce.Plugin.Catalog.Catalog>()))
                {
                    if (itemId.StartsWith(CommerceEntity.IdPrefix<SellableItem>())) return CatalogConstants.CatalogToSellableItem;
                    else if (itemId.StartsWith(CommerceEntity.IdPrefix<Category>())) return CatalogConstants.CatalogToCategory;
                }
                else if (parentId.StartsWith(CommerceEntity.IdPrefix<Category>()))
                {
                    if (itemId.StartsWith(CommerceEntity.IdPrefix<SellableItem>())) return CatalogConstants.CategoryToSellableItem;
                    else if (itemId.StartsWith(CommerceEntity.IdPrefix<Category>())) return CatalogConstants.CategoryToCategory;
                }
                throw new Exception($"Error, can not determine relationship type between ItemId '{itemId}' and ParentId '{parentId}'.");
            }
        }
    }
}
