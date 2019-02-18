using Sitecore.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class TransientImportDataPolicy : Policy
    {
        public IList<string> ParentCatalogNameList { get; set; }
        public IList<string> ParentCategoryNameList { get; set; }
        public IList<string> ImageNameList { get; set; }
    }
}
