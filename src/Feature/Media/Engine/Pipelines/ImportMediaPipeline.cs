using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Media.Engine
{
    public class ImportMediaPipeline : CommercePipeline<string, string>, IImportMediaPipeline
    {
        public ImportMediaPipeline(IPipelineConfiguration<IImportMediaPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}