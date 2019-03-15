using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class ImportCategoriesFromFileBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private CommerceCommander CommerceCommander { get; set; }

        public ImportCategoriesFromFileBlock(
            IServiceProvider serviceProvider)
        {
            CommerceCommander = serviceProvider.GetService<CommerceCommander>();
        }

        private void LogInitialization(CommercePipelineExecutionContext context, ImportCategoriesPolicy policy)
        {
            var log = new StringBuilder();
            log.AppendLine($"{System.Environment.NewLine}");
            log.AppendLine($"{GetType().Name} - Starting");
            log.AppendLine($"Settings:");
            log.AppendLine($"\t {nameof(policy.ItemsPerBatch)} = {policy.ItemsPerBatch}");
            log.AppendLine($"\t {nameof(policy.SleepBetweenBatches)} = {policy.SleepBetweenBatches}");
            log.AppendLine($"\t {nameof(policy.FileFolderPath)}  = {policy.FileFolderPath}");
            log.AppendLine($"\t {nameof(policy.FileArchiveFolderPath)}  = {policy.FileArchiveFolderPath}");
            log.AppendLine($"\t {nameof(policy.FilePrefix)}  = {policy.FilePrefix}");
            log.AppendLine($"\t {nameof(policy.FileExtention)}  = {policy.FileExtention}");
            log.AppendLine($"\t {nameof(policy.FileRecordSeparator)}  = {policy.FileRecordSeparator}");

            context.Logger.LogInformation(log.ToString());
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var importPolicy = context.GetPolicy<ImportCategoriesPolicy>();
            
            LogInitialization(context, importPolicy);

            try
            {
                var filePath = CommerceCommander.Command<GetFileCommand>().Process(context.CommerceContext, importPolicy.FileFolderPath, importPolicy.FilePrefix, importPolicy.FileExtention);
                if (string.IsNullOrEmpty(filePath))
                {
                    context.Logger.LogInformation($"{Name} - Skipping execution as there are no files to process.");
                    return null;
                }

                using (var reader = new StreamReader(filePath))
                {
                    // skip header
                    if (!reader.EndOfStream) reader.ReadLine();



                    while (!reader.EndOfStream)
                    {
                        var importRawLines = new List<string[]>();
                        for (int i = 0; !reader.EndOfStream || i >= importPolicy.ItemsPerBatch; i++)
                        {
                            importRawLines.Add(reader.ReadLine().Split(new string[] { importPolicy.FileGroupSeparator }, new StringSplitOptions()));
                        }

                        var importItems = await CommerceCommander.Command<TransformImportToCategoryCommand>().Process(context.CommerceContext, importRawLines);


                        // TODO: var existingItems = await CommerceCommander.Command<GetSellableItemsBulkCommand>().Process(context.CommerceContext, importItems);

                        // TODO: var newItems = importItems.Except(existingItems, sellableItemComparerByProductId);
                        // TODO: var changedItems = existingItems.Except(importItems, sellableItemComparerByImportData);

                        // TODO: await CommerceCommander.Command<CopyImportToSellableItemsCommand>().Process(context.CommerceContext, importItems, changedItems);

                        var associationsToCreate = importItems.SelectMany(i => i.GetPolicy<TransientImportCategoryDataPolicy>().ParentAssociationsToCreateList).ToList();
                        var associationsToRemove = importItems.SelectMany(i => i.GetPolicy<TransientImportCategoryDataPolicy>().ParentAssociationsToRemoveList).ToList();

                        RemoveTransientData(importItems);

                        await CommerceCommander.Command<PersistEntityBulkCommand>().Process(context.CommerceContext, importItems);
                        await CommerceCommander.Command<AssociateToParentBulkCommand>().Process(context.CommerceContext, associationsToCreate);
                        // TODO: complete this command
                        await CommerceCommander.Command<DisassociateToParentBulkCommand>().Process(context.CommerceContext, associationsToRemove); 

                        await Task.Delay(importPolicy.SleepBetweenBatches);
                    }
                }

                CommerceCommander.Command<MoveFileCommand>().Process(context.CommerceContext, importPolicy.FileArchiveFolderPath, filePath);
            }
            catch (Exception ex)
            {
                context.Abort(await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    Name,
                    new object[1] { ex },
                    $"{Name} Import Exception: {ex.Message}"),
                    context);
            }

            return null;
        }

        private void RemoveTransientData(IEnumerable<Category> importItems)
        {
            foreach (var item in importItems)
            {
                if (item.HasPolicy<TransientImportCategoryDataPolicy>())
                    item.RemovePolicy(typeof(TransientImportCategoryDataPolicy));
            }
        }
    }
}
