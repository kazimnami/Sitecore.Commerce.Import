using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Import.Engine
{
    public interface IImportDataPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
    }
}