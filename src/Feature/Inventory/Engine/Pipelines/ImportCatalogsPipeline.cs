using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Inventory.Engine
{
    public class ImportInventoryPipeline : CommercePipeline<string, string>, IImportInventoryPipeline
    {
        public ImportInventoryPipeline(IPipelineConfiguration<IImportInventoryPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}