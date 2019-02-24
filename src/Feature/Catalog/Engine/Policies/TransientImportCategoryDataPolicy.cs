using Sitecore.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class TransientImportCategoryDataPolicy : Policy
    {
        //public IList<CategoryAssociationModel> CategoryAssociationList { get; set; } = new List<CategoryAssociationModel>();

        public IList<ParentAssociationModel> ParentAssociationsToCreateList { get; set; } = new List<ParentAssociationModel>();
        //public IList<ParentAssociationModel> ParentAssociationsToRemoveList { get; set; } = new List<ParentAssociationModel>();

    }
}
