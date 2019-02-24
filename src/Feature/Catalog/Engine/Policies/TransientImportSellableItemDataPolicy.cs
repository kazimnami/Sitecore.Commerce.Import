using Sitecore.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class TransientImportSellableItemDataPolicy : Policy
    {
        public IList<CatalogAssociationModel> CatalogAssociationList { get; set; } = new List<CatalogAssociationModel>();
        public IList<CategoryAssociationModel> CategoryAssociationList { get; set; } = new List<CategoryAssociationModel>();
        public IList<string> ImageNameList { get; set; } = new List<string>();

        public IList<ParentAssociationModel> ParentAssociationsToCreateList { get; set; } = new List<ParentAssociationModel>();
        public IList<ParentAssociationModel> ParentAssociationsToRemoveList { get; set; } = new List<ParentAssociationModel>();

    }
}
