using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ManagedLists;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Feature.Catalog.Engine
{
    public class TransformImportToCategoryCommand : CommerceCommand
    {
        private const int CatalogNameIndex = 0;
        private const int CategoryNameIndex = 1;
        private const int ParentCategoryNameIndex = 2;
        private const int CategoryDisplayNameIndex = 3;

        public TransformImportToCategoryCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public IEnumerable<Category> Process(CommerceContext commerceContext, IEnumerable<string[]> importRawLines)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var importPolicy = commerceContext.GetPolicy<ImportCategoriesPolicy>();
                var importItems = new List<Category>();
                foreach (var rawFields in importRawLines)
                {
                    var item = new Category();
                    TransformCore(commerceContext, rawFields, item);
                    TransformTransientData(importPolicy, rawFields, item);
                    importItems.Add(item);
                }

                return importItems;
            }
        }

        private void TransformCore(CommerceContext commerceContext, string[] rawFields, Category item)
        {
            var catalogName = rawFields[CatalogNameIndex];
            item.Name = rawFields[CategoryNameIndex];
            item.Id = item.Name.ToCategoryId(catalogName);
            item.FriendlyId = item.Name.ToCategoryFriendlyId(catalogName);
            item.SitecoreId = GuidUtils.GetDeterministicGuidString(item.Id);
            item.DisplayName = rawFields[CategoryDisplayNameIndex];
            //item.Description = arg.Description;
            var component = item.GetComponent<ListMembershipsComponent>();
            component.Memberships.Add(string.Format("{0}", CommerceEntity.ListName<Category>()));
            component.Memberships.Add(commerceContext.GetPolicy<KnownCatalogListsPolicy>().CatalogItems);
        }

        private void TransformTransientData(ImportCategoriesPolicy importPolicy, string[] rawFields, Category item)
        {
            var catalogName = rawFields[CatalogNameIndex];
            var data = new TransientImportCategoryDataPolicy();

            var categoryRecord = rawFields[ParentCategoryNameIndex].Split(new string[] { importPolicy.FileRecordSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var categoryUnit in categoryRecord)
            {
                //data.CategoryAssociationList.Add(new CategoryAssociationModel { CatalogName = catalogName, CategoryName = categoryUnit });
                data.ParentAssociationsToCreateList.Add(new ParentAssociationModel (catalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>(), categoryUnit.ToCategoryId(catalogName)));
            }

            if(data.ParentAssociationsToCreateList.Count().Equals(0))
            {
                data.ParentAssociationsToCreateList.Add(new ParentAssociationModel (catalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>(), catalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>()));
            }

            item.Policies.Add(data);
        }
    }
}
