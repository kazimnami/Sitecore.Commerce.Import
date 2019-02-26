using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Feature.Catalog.Engine
{
    public class TransientImportCategoryDataPolicy : Policy
    {
        public IList<CatalogAssociationModel> CatalogAssociationList { get; set; } = new List<CatalogAssociationModel>();
        public IList<CategoryAssociationModel> CategoryAssociationList { get; set; } = new List<CategoryAssociationModel>();
        public IList<ParentAssociationModel> ParentAssociationsToCreateList { get; set; } = new List<ParentAssociationModel>();
        public IList<ParentAssociationModel> ParentAssociationsToRemoveList { get; set; } = new List<ParentAssociationModel>();
    }
}
