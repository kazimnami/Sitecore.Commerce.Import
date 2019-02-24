using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class PersistEntityBulkCommand : CommerceCommand
    {
        public PersistEntityBulkCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<bool> Process(CommerceContext commerceContext, IEnumerable<CommerceEntity> items)
        {
            using (CommandActivity.Start(commerceContext, this))
            {

                foreach (var item in items)
                {
                    await PerformTransaction(commerceContext, async () =>
                    {
                        var arg = new PersistEntityArgument(item);
                        await Pipeline<IPersistEntityPipeline>().Run(arg, commerceContext.GetPipelineContextOptions());
                    });
                }

                return true;
            }
        }
    }
}
