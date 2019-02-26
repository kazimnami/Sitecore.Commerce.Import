using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ManagedLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class TransformImportToCategoryCommand : CommerceCommand
    {
        private const int CatalogNameIndex = 0;
        private const int CategoryNameIndex = 1;
        private const int ParentCategoryNameIndex = 2;
        private const int CategoryDisplayNameIndex = 3;

        public TransformImportToCategoryCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<IEnumerable<Category>> Process(CommerceContext commerceContext, IEnumerable<string[]> importRawLines)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var importPolicy = commerceContext.GetPolicy<ImportCategoriesPolicy>();
                var importItems = new List<Category>();
                var transientDataList = new List<TransientImportCategoryDataPolicy>();
                foreach (var rawFields in importRawLines)
                {
                    var item = new Category();
                    TransformCore(commerceContext, rawFields, item);
                    TransformTransientData(importPolicy, rawFields, item, transientDataList);
                    importItems.Add(item);
                }

                await TransformCatalog(commerceContext, importItems);
                await TransformCategory(commerceContext, transientDataList, importItems);

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

        private void TransformTransientData(ImportCategoriesPolicy importPolicy, string[] rawFields, Category item, List<TransientImportCategoryDataPolicy> transientDataList)
        {
            var catalogName = rawFields[CatalogNameIndex];
            var data = new TransientImportCategoryDataPolicy();

            var categoryRecord = rawFields[ParentCategoryNameIndex].Split(new string[] { importPolicy.FileRecordSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var categoryUnit in categoryRecord)
            {
                data.ParentAssociationsToCreateList.Add(new ParentAssociationModel(item.Id, catalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>(), categoryUnit.ToCategoryId(catalogName)));
                data.CategoryAssociationList.Add(new CategoryAssociationModel { CatalogName = catalogName, CategoryName = categoryUnit });
            }

            if (data.CategoryAssociationList.Count().Equals(0))
            {
                data.ParentAssociationsToCreateList.Add(new ParentAssociationModel(item.Id, catalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>(), catalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>()));
                data.CatalogAssociationList.Add(new CatalogAssociationModel { Name = catalogName, IsParent = true });
            }
            else
            {
                data.CatalogAssociationList.Add(new CatalogAssociationModel { Name = catalogName });
            }

            item.Policies.Add(data);
            transientDataList.Add(data);
        }

        private async Task TransformCatalog(CommerceContext commerceContext, List<Category> importItems)
        {
            var allCatalogs = commerceContext.GetObject<IEnumerable<Sitecore.Commerce.Plugin.Catalog.Catalog>>();
            if (allCatalogs == null)
            {
                allCatalogs = await Command<GetCatalogsCommand>().Process(commerceContext);
                commerceContext.AddObject(allCatalogs);
            }

            var allCatalogsDictionary = allCatalogs.ToDictionary(c => c.Name);

            foreach (var item in importItems)
            {
                var transientData = item.GetPolicy<TransientImportCategoryDataPolicy>();

                var catalogList = new List<string>();
                foreach (var association in transientData.CatalogAssociationList)
                {
                    allCatalogsDictionary.TryGetValue(association.Name, out Sitecore.Commerce.Plugin.Catalog.Catalog catalog);
                    if (catalog != null)
                    {
                        catalogList.Add(catalog.SitecoreId);
                    }
                    else
                    {
                        commerceContext.Logger.LogWarning($"Warning, Category with id '{item.Id}' attempting import into catalog '{association.Name}' which doesn't exist.");
                    }
                }

                var parentCatalogList = new List<string>();
                foreach (var association in transientData.ParentAssociationsToCreateList)
                {
                    if (association.ParentId.IsEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>())
                    {
                        allCatalogsDictionary.TryGetValue(association.ParentId.RemoveIdPrefix<Sitecore.Commerce.Plugin.Catalog.Catalog>(), out Sitecore.Commerce.Plugin.Catalog.Catalog catalog);
                        if (catalog != null)
                        {
                            parentCatalogList.Add(catalog.SitecoreId);
                        }
                        else
                        {
                            commerceContext.Logger.LogWarning($"Warning, Category with id {item.Id} attempting import into catalog {association.ParentId} which doesn't exist.");
                        }
                    }
                }

                item.CatalogToEntityList = catalogList.Any() ? string.Join("|", catalogList) : null; 
                item.ParentCatalogList = parentCatalogList.Any() ? string.Join("|", parentCatalogList) : null;
            }
        }

        private async Task TransformCategory(CommerceContext commerceContext, List<TransientImportCategoryDataPolicy> transientDataList, List<Category> importItems)
        {
            var catalogNameList = transientDataList.SelectMany(d => d.CatalogAssociationList).Select(a => a.Name).Distinct();
            var catalogContextList = await Command<GetCatalogContextCommand>().Process(commerceContext, catalogNameList, false);

            foreach (var item in importItems)
            {
                var transientData = item.GetPolicy<TransientImportCategoryDataPolicy>();

                var itemsCategoryList = new List<string>();
                foreach (var categoryAssociation in transientData.CategoryAssociationList)
                {
                    var catalogContext = catalogContextList.FirstOrDefault(c => c.CatalogName.Equals(categoryAssociation.CatalogName));
                    if (catalogContext == null) throw new Exception($"Error, catalog not found {categoryAssociation.CatalogName}. This should not happen.");

                    // Find category, existing
                    catalogContext.CategoriesByName.TryGetValue(categoryAssociation.CategoryName, out Category category);
                    if (category == null)
                    {
                        // Find category, new
                        category = importItems.FirstOrDefault(c => c.Name.Equals(categoryAssociation.CategoryName));
                    }

                    if (category != null)
                    {
                        // Found category
                        itemsCategoryList.Add(category.SitecoreId);
                        transientData.ParentAssociationsToCreateList.Add(new ParentAssociationModel(item.Id, categoryAssociation.CatalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>(), category));
                    }
                    else
                    {
                        commerceContext.Logger.LogWarning($"Warning, Category with id {item.Id} attempting import into category {categoryAssociation.CategoryName} which doesn't exist.");
                    }
                }

                item.ParentCategoryList = itemsCategoryList.Any() ? string.Join("|", itemsCategoryList) : null;
            }
        }
    }
}
