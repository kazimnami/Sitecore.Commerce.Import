using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feature.Import.Engine
{
    public class DataImportMinion : Minion
    {
        private static int isRunning = 0;

        private CommerceCommander CommerceCommander { get; set; }

        public override void Initialize(IServiceProvider serviceProvider, ILogger logger, MinionPolicy policy, CommerceEnvironment environment, CommerceContext globalContext)
        {
            base.Initialize(serviceProvider, logger, policy, environment, globalContext);
            CommerceCommander = serviceProvider.GetService<CommerceCommander>();
            LogInitialization();
        }

        private void LogInitialization()
        {
            var log = new StringBuilder();
            log.Append($"{Name} settings:{System.Environment.NewLine}");
            log.AppendLine($"\t WakeupInterval = {Policy.WakeupInterval}");
            log.AppendLine($"\t ListToWatch = {Policy.ListToWatch}");
            log.AppendLine($"\t ItemsPerBatch {Policy.ItemsPerBatch}");
            log.AppendLine($"\t SleepBetweenBatches {Policy.SleepBetweenBatches}");
            Logger.LogInformation(log.ToString());
        }

        public override async Task<MinionRunResultsModel> Run()
        {
            var minion = this;

            if (Interlocked.CompareExchange(ref isRunning, 1, 0) != 0)
            {
                //minion.Logger.LogInformation($"{minion.Name} - Skipping execution");
                return new MinionRunResultsModel();
            }

            try
            {
                minion.Logger.LogInformation($"{minion.Name} - Starting");

                var commerceContext = new CommerceContext(minion.Logger, minion.MinionContext.TelemetryClient, null) { Environment = minion.Environment };
                await CommerceCommander.Pipeline<IImportDataPipeline>().Run(null, commerceContext.GetPipelineContextOptions());

                minion.Logger.LogInformation($"{minion.Name} - Finished");

                return new MinionRunResultsModel();
            }
            catch (Exception ex)
            {
                Logger.LogError($"{Name}-Error occured: {ex.Message}", ex);
                throw;
            }
            finally
            {
                isRunning = 0;
            }
        }
    }
}
