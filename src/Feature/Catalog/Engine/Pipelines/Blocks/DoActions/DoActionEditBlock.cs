using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    [PipelineDisplayName(Constants.Pipelines.Blocks.ProductExtensionDoActionEditBlock)]
    public class DoActionEditBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionEditBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var notesActionsPolicy = context.GetPolicy<KnownProductExtensionActionsPolicy>();

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(arg.Action) || !arg.Action.Equals(notesActionsPolicy.ProductExtensionEdit, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(arg);
            }

            // Get the sellable item from the context
            var entity = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(arg.EntityId));
            if (entity == null)
            {
                return Task.FromResult(arg);
            }

            // Get the component from the sellable item or its variation
            ProductExtensionComponent component;
            if (arg.ItemId != string.Empty)
            {
                component = GetViewBlock.GetProductExtensionComponent(entity, arg.ItemId);
            }
            else
            {
                component = entity.GetComponent<ProductExtensionComponent>();
            }

            // Map entity view properties to component
            component.GetPropertiesFromView(arg);

            // Persist changes
            this._commerceCommander.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(entity), context);

            return Task.FromResult(arg);
        }
    }
}
