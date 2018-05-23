using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Catalog.Engine
{
    [PipelineDisplayName(Constants.Pipelines.Blocks.ProductExtensionPopulateActionsBlock)]
    public class PopulateActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

			//if (arg.Name.Equals("ToolsNavigation", StringComparison.OrdinalIgnoreCase))
			//{
			//	arg.ChildViews.Remove(arg.ChildViews[0]);
			//	return Task.FromResult(arg);
			//}

			var viewsPolicy = context.GetPolicy<KnownProductExtensionViewsPolicy>();

            if (string.IsNullOrEmpty(arg?.Name) || 
                !arg.Name.Equals(viewsPolicy.ProductExtension, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(arg);
            }

            var actionPolicy = arg.GetPolicy<ActionsPolicy>();

            actionPolicy.Actions.Add(
                new EntityActionView
                {
                    Name = context.GetPolicy<KnownProductExtensionActionsPolicy>().ProductExtensionEdit,
                    DisplayName = "Edit Sellable Item Extension Details",
                    Description = "Edits the sellable item extension details",
                    IsEnabled = true,
                    EntityView = arg.Name,
                    Icon = "edit"
                });

            return Task.FromResult(arg);
        }
    }
}
