using Sitecore.Commerce.Core;

namespace Foundation.Import.Engine
{
    public class ImportPolicy : Policy
    {
        public int ItemsPerBatch { get; set; } = 1000;
        public int SleepBetweenBatches { get; set; } = 30000;
        public string FileFolderPath { get; set; } = @"data\Import";
        public string FileArchiveFolderPath { get; set; } = @"data\Import\Archive";
        public string FilePrefix { get; set; } = "Import";
        public string FileExtention { get; set; } = "CSV";
        public char FileGroupSeparator { get; set; } = ',';
        public string FileRecordSeparator { get; set; } = "^^";
        public string FileUnitSeparator { get; set; } = "^-";
        
    }
}
