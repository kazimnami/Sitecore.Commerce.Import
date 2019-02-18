using Microsoft.AspNetCore.Hosting;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class GetFileCommand : CommerceCommand
    {
        private readonly IHostingEnvironment HostingEnvironment;

        public GetFileCommand(IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment) : base(serviceProvider)
        {
            this.HostingEnvironment = hostingEnvironment;
        }

        public string Process(CommerceContext commerceContext, string folderPath, string filePrefix, string fileExtention)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                //string directoryPath = Path.Combine(this.HostingEnvironment.WebRootPath, folderPath);
                string directoryPath = @"C:\Import";

                var directoryInfo = Directory.CreateDirectory(directoryPath);
                var fileInfoList = directoryInfo.GetFileSystemInfos()
                    .OrderBy(fi => fi.CreationTime);

                foreach (FileSystemInfo fileInfo in fileInfoList)
                {
                    if (fileInfo.Name.StartsWith(filePrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // Only pick the file up when it's finished. 
                        // Once the creation job completes it should modify the extension to signal it's completion
                        if (fileInfo.Extension.EndsWith(fileExtention, StringComparison.InvariantCulture))
                        {
                            return fileInfo.FullName;
                        }

                        // we are only interested in the oldest file that matches our criteria. 
                        break;
                    }
                }
            }
            return null;
        }
    }
}
