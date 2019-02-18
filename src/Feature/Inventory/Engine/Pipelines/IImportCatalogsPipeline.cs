using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Inventory.Engine
{
    public interface IImportInventoryPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
    }
}