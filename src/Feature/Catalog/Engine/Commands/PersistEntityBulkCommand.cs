using Microsoft.Extensions.Logging;
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
                commerceContext.Logger.LogInformation($"Called - {nameof(PersistEntityBulkCommand)}.");

                foreach (var item in items)
                {
                    // Need to clear message as any prior error will cause all transactions to abort.
                    commerceContext.ClearMessages();

                    await PerformTransaction(commerceContext, async () =>
                    {
                        var arg = new PersistEntityArgument(item);
                        await Pipeline<IPersistEntityPipeline>().Run(arg, commerceContext.GetPipelineContextOptions());
                    });
                }

                commerceContext.Logger.LogInformation($"Completed - {nameof(PersistEntityBulkCommand)}.");

                return true;
            }
        }
    }
}
