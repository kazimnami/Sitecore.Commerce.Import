using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using System.Reflection;

namespace Feature.Import.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .AddPipeline<IImportDataPipeline, ImportDataPipeline>(p => p
                    //.Add<Catalog.Engine.IImportCatalogsPipeline>()
                    .Add<Catalog.Engine.ImportCategoriesFromFileBlock>()
                    //.Add<Media.Engine.IImportMediaPipeline>()
                    .Add<Catalog.Engine.ImportSellableItemsFromFileBlock>()
                    .Add<Inventory.Engine.ImportInventoryFromFileBlock>()
                )
            );
        }
    }
}
