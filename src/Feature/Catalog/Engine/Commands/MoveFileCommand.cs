using Microsoft.AspNetCore.Hosting;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using System;
using System.IO;

namespace Feature.Catalog.Engine
{
    public class MoveFileCommand : CommerceCommand
    {
        private readonly IHostingEnvironment HostingEnvironment;

        public MoveFileCommand(IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment) : base(serviceProvider)
        {
            HostingEnvironment = hostingEnvironment;
        }

        public string Process(CommerceContext commerceContext, string directoryPath, string filePath)
        {
            using (CommandActivity.Start(commerceContext, this))
            {

                var file = new FileInfo(filePath);
                //string destinationFilePath = Path.Combine(this.HostingEnvironment.WebRootPath, directoryPath, file.Name);
                directoryPath = @"c:\Import\Archive"; 
                string destinationFilePath = Path.Combine(directoryPath, file.Name);

                var directoryInfo = Directory.CreateDirectory(directoryPath);

                File.Move(filePath, destinationFilePath);

                return null;
            }
        }
    }
}
