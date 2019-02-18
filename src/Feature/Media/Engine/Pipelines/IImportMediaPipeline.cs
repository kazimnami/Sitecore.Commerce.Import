using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Media.Engine
{
    public interface IImportMediaPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
    }
}