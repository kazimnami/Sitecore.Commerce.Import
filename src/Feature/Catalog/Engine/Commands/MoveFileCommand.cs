using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Conditions;

namespace Feature.Catalog.Engine
{
    public class MoveFileCommand : CommerceCommand
    {
        private readonly IHostingEnvironment HostingEnvironment;

        public MoveFileCommand(IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment) : base(serviceProvider)
        {
            this.HostingEnvironment = hostingEnvironment;
        }

        public string Process(CommerceContext commerceContext, string directoryPath, string filePath)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var file = new FileInfo(filePath);
                //string destinationFilePath = Path.Combine(this.HostingEnvironment.WebRootPath, directoryPath, file.Name);
                string destinationFilePath = Path.Combine(@"c:\Import\Archive", file.Name);

                var directoryInfo = Directory.CreateDirectory(directoryPath);

                File.Move(filePath, destinationFilePath);
            }
            return null;
        }
    }
}
