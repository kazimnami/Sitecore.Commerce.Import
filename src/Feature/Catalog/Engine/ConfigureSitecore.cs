using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Feature.Catalog.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);
            services.RegisterAllCommands(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IGetEntityViewPipeline>(c =>
                {
                    c.Add<GetViewBlock>().After<GetSellableItemDetailsViewBlock>();
                })
                .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(c =>
                {
                    c.Add<PopulateActionsBlock>().After<InitializeEntityViewActionsBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(c =>
                {
                    c.Add<DoActionEditBlock>().After<ValidateEntityVersionBlock>();
                })
            );
        }
    }
}
