using Foundation.Import.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature.Inventory.Engine
{
    public class ImportInventoryFromFileBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private CommerceCommander CommerceCommander { get; set; }

        public ImportInventoryFromFileBlock(
            IServiceProvider serviceProvider)
        {
            CommerceCommander = serviceProvider.GetService<CommerceCommander>();
        }

        private void LogInitialization(CommercePipelineExecutionContext context, ImportInventoryPolicy policy)
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
            var importPolicy = context.GetPolicy<ImportInventoryPolicy>();
            var comparerById = new ImportInventoryComparer(InventoryComparerConfiguration.ById);
            var comparerByData = new ImportInventoryComparer(InventoryComparerConfiguration.ByData);

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
                    using (var parser = new NotVisualBasic.FileIO.CsvTextFieldParser(reader))
                    {
                        parser.SetDelimiter(importPolicy.FileGroupSeparator);
                        // skip header
                        if (!parser.EndOfData) parser.ReadFields();

                        while (!parser.EndOfData)
                        {
                            var importRawLines = new List<string[]>();
                            for (int i = 0; !parser.EndOfData && i <= importPolicy.ItemsPerBatch - 1; i++)
                            {
                                importRawLines.Add(parser.ReadFields());
                            }

                            var importItems = await CommerceCommander.Command<TransformImportToInventoryInformationCommand>().Process(context.CommerceContext, importRawLines);
                            var existingItems = await CommerceCommander.Command<GetInventoryBulkCommand>().Process(context.CommerceContext, importItems);

                            var newItems = importItems.Except(existingItems, comparerById).ToList();
                            var changedItems = existingItems.Except(importItems, comparerByData).ToList();

                            await CommerceCommander.Command<CopyImportToInventoryCommand>().Process(context.CommerceContext, importItems, changedItems);

                            var associationsToCreate = newItems.Select(i => new ParentAssociationModel(i.Id, i.InventorySet.EntityTarget)).ToList();

                            await CommerceCommander.Command<PersistEntityBulkCommand>().Process(context.CommerceContext, newItems.Union(changedItems));
                            await CommerceCommander.Command<AssociateToParentBulkCommand>().Process(context.CommerceContext, associationsToCreate);
                            await CommerceCommander.Command < AssociateInventoryToSellableItemCommand>().Process(context.CommerceContext, newItems);

                            await Task.Delay(importPolicy.SleepBetweenBatches);
                        }
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
    }
}
