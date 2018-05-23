using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Catalog.Engine
{
    [PipelineDisplayName(Constants.Pipelines.Blocks.ProductExtensionGetViewBlock)]
    public class GetViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander _viewCommander;

        public GetViewBlock(ViewCommander viewCommander)
        {
            this._viewCommander = viewCommander;
        }

        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var request = this._viewCommander.CurrentEntityViewArgument(context.CommerceContext);

            var catalogViewsPolicy = context.GetPolicy<KnownCatalogViewsPolicy>();

            var productExtensionViewsPolicy = context.GetPolicy<KnownProductExtensionViewsPolicy>();
            var productExtensionActionsPolicy = context.GetPolicy<KnownProductExtensionActionsPolicy>();

            var isVariationView = request.ViewName.Equals(catalogViewsPolicy.Variant, StringComparison.OrdinalIgnoreCase);
            var isConnectView = arg.Name.Equals(catalogViewsPolicy.ConnectSellableItem, StringComparison.OrdinalIgnoreCase);

            // Make sure that we target the correct views
            if (string.IsNullOrEmpty(request.ViewName) ||
                !request.ViewName.Equals(catalogViewsPolicy.Master, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(catalogViewsPolicy.Details, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(productExtensionViewsPolicy.ProductExtension, StringComparison.OrdinalIgnoreCase) &&
                !isVariationView &&
                !isConnectView)
            {
                return Task.FromResult(arg);
            }

            // Only proceed if the current entity is a sellable item
            if (!(request.Entity is SellableItem))
            {
                return Task.FromResult(arg);
            }

            var sellableItem = (SellableItem)request.Entity;

            // See if we are dealing with the base sellable item or one of its variations.
            var variationId = string.Empty;
            if ((isVariationView && !string.IsNullOrEmpty(arg.ItemId)) || request.Entity.FriendlyId != arg.ItemId)
            {
                variationId = arg.ItemId;
            }

            var targetView = arg;

            // Check if the edit action was requested
            var isEditView = !string.IsNullOrEmpty(arg.Action) && arg.Action.Equals(productExtensionActionsPolicy.ProductExtensionEdit, StringComparison.OrdinalIgnoreCase);
            if (!isEditView)
            {
                // Create a new view and add it to the current entity view.
                var view = new EntityView
                {
                    Name = context.GetPolicy<KnownProductExtensionViewsPolicy>().ProductExtension,
                    DisplayName = "Details Extension",
                    EntityId = arg.EntityId,
                    ItemId = variationId,
                    DisplayRank = 500
                };

                arg.ChildViews.Add(view);

                targetView = view;
            }

            if (sellableItem != null && (sellableItem.HasComponent<ProductExtensionComponent>(variationId) || isConnectView || isEditView))
            {
                ProductExtensionComponent component;
                if (variationId != string.Empty)
                {
                    component = GetProductExtensionComponent(sellableItem, variationId);                }
                else
                {
                    component = sellableItem.GetComponent<ProductExtensionComponent>();
                }

                component.AddPropertiesToView(targetView, !isEditView);
            }

            return Task.FromResult(arg);
        }

        public static ProductExtensionComponent GetProductExtensionComponent(SellableItem instance, string variationId)
        {
            ItemVariationComponent variation = instance.GetVariation(variationId);

            if (variation.HasComponent<ProductExtensionComponent>() == false)
            {
                var component = instance.GetComponent<ProductExtensionComponent>().Copy();
                variation.SetComponent(component);

                return component;
            }
            else
            {
                return variation.GetComponent<ProductExtensionComponent>();
            }
        }
    }
}
