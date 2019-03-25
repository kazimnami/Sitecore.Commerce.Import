using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class TransformImportToSellableItemsCommand : CommerceCommand
    {
        // Core
        private const int ProductIdIndex = 0;
        private const int ProductNameIndex = 1;
        private const int DisplayNameIndex = 2;
        private const int DescriptionIndex = 3;
        private const int BrandIndex = 4;
        private const int ManufacturerIndex = 5;
        private const int TypeOfGoodIndex = 6;
        private const int TagsIndex = 7;
        private const int ListPriceIndex = 8;
        private const int ImagesIndex = 9;
        private const int CatalogNameIndex = 10;
        private const int CategoryNameIndex = 11;
        // Product Extension Component
        private const int StyleIndex = 12;
        private const int FuelTypeIndex = 13;
        private const int NaturalGasConversionAvailableIndex = 14;
        private const int DimensionsHeightHoodOpenIndex = 15;
        private const int DimensionsHeightHoodClosedIndex = 16;
        private const int DimensionsWidthIndex = 17;
        private const int DimensionsDepthIndex = 18;
        // Limit
        private const int ExpectedIndexLimit = 19;
        // List Price
        private const int ListPriceCurrencyCodeIndex = 0;
        private const int ListPriceAmountIndex = 1;



        public TransformImportToSellableItemsCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<IEnumerable<SellableItem>> Process(CommerceContext commerceContext, IEnumerable<string[]> importRawLines)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var importPolicy = commerceContext.GetPolicy<ImportSellableItemsPolicy>();
                var importItems = new List<SellableItem>();
                var transientDataList = new List<TransientImportSellableItemDataPolicy>();
                foreach (var rawFields in importRawLines)
                {
                    if (rawFields.Count() != ExpectedIndexLimit)
                    {
                        commerceContext.Logger.LogWarning($"Warning, skipping unexpected line with field count'{rawFields.Count()}' and expected '{ExpectedIndexLimit}'. Line '{string.Join(",", rawFields)}'");
                        continue;
                    }

                    var item = new SellableItem();
                    TransformCore(commerceContext, rawFields, item);
                    TransformListPrice(commerceContext, importPolicy, rawFields, item);
                    TransformProductExtension(rawFields, item);
                    TransformTransientData(importPolicy, rawFields, item, transientDataList);
                    importItems.Add(item);
                }

                await TransformCatalog(commerceContext, importItems);
                await TransformCategory(commerceContext, transientDataList, importItems);
                await TransformImages(commerceContext, transientDataList, importItems);

                return importItems;
            }
        }

        private void TransformCore(CommerceContext commerceContext, string[] rawFields, SellableItem item)
        {
            item.ProductId = rawFields[ProductIdIndex];
            item.Id = item.ProductId.ToEntityId<SellableItem>();
            item.FriendlyId = item.ProductId;
            item.Name = rawFields[ProductNameIndex];
            /// Be sure not to include SitecoreId in <see cref="ProductExtensionComponentComparer"/> 
            item.SitecoreId = GuidUtils.GetDeterministicGuidString(item.Id);
            item.DisplayName = rawFields[DisplayNameIndex];
            item.Description = rawFields[DescriptionIndex];
            item.Brand = rawFields[BrandIndex];
            item.Manufacturer = rawFields[ManufacturerIndex];
            item.TypeOfGood = rawFields[TypeOfGoodIndex];
            var tags = string.IsNullOrEmpty(rawFields[TagsIndex]) ? null : rawFields[TagsIndex].Split('|');
            item.Tags = tags == null ? new List<Tag>() : tags.Select(x => new Tag(x)).ToList();

            var component = item.GetComponent<ListMembershipsComponent>();
            component.Memberships.Add(string.Format("{0}", CommerceEntity.ListName<SellableItem>()));
            component.Memberships.Add(commerceContext.GetPolicy<KnownCatalogListsPolicy>().CatalogItems);
        }

        private void TransformListPrice(CommerceContext commerceContext, ImportSellableItemsPolicy importPolicy, string[] rawFields, SellableItem item)
        {
            var listPrices = rawFields[ListPriceIndex].Split(new string[] { importPolicy.FileRecordSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (listPrices == null || listPrices.Count() == 0)
            {
                commerceContext.Logger.LogWarning($"Warning, Product with id '{item.ProductId}' does not contain a list price. Has value '{rawFields[ListPriceIndex]}'");
                return;
            }

            var priceList = new List<Money>();
            foreach (var listPrice in listPrices)
            {
                var priceData = listPrice.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (priceData.Count() != 2)
                {
                    commerceContext.Logger.LogWarning($"Warning, Product with id '{item.ProductId}' does not contain a list price. Has value '{rawFields[ListPriceIndex]}'");
                    continue;
                }
                priceList.Add(new Money
                {
                    CurrencyCode = priceData[ListPriceCurrencyCodeIndex],
                    Amount = decimal.Parse(priceData[ListPriceAmountIndex])
                });
            }

            if (!priceList.Any()) return;

            item.Policies.Add(new ListPricingPolicy(priceList));
        }

        private void TransformProductExtension(string[] rawFields, SellableItem item)
        {
            //item.Components.Add(new ProductExtensionComponent
            //{
            //    Style = rawFields[StyleIndex],
            //    FuelType = rawFields[FuelTypeIndex],
            //    NaturalGasConversionAvailable = rawFields[NaturalGasConversionAvailableIndex],
            //    DimensionsHeightHoodOpen = rawFields[DimensionsHeightHoodOpenIndex],
            //    DimensionsHeightHoodClosed = rawFields[DimensionsHeightHoodClosedIndex],
            //    DimensionsWidth = rawFields[DimensionsWidthIndex],
            //    DimensionsDepth = rawFields[DimensionsDepthIndex]
            //});
        }

        private void TransformTransientData(ImportSellableItemsPolicy importPolicy, string[] rawFields, SellableItem item, List<TransientImportSellableItemDataPolicy> transientDataList)
        {
            var data = new TransientImportSellableItemDataPolicy
            {
                ImageNameList = rawFields[ImagesIndex].Split(new string[] { importPolicy.FileRecordSeparator }, StringSplitOptions.None)
            };

            var catalogRecord = rawFields[CatalogNameIndex].Split(new string[] { importPolicy.FileRecordSeparator }, StringSplitOptions.None).ToList();
            foreach (var catalogUnit in catalogRecord)
            {
                data.CatalogAssociationList.Add(new CatalogAssociationModel { Name = catalogUnit });
            }

            var catalogCategoryRecord = rawFields[CategoryNameIndex].Split(new string[] { importPolicy.FileRecordSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var catalogCategoryUnit in catalogCategoryRecord)
            {
                var units = catalogCategoryUnit.Split(new string[] { importPolicy.FileUnitSeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (units == null || units.Count() != 2) throw new Exception("Error, unexpected value in CategoryNameIndex");
                data.CategoryAssociationList.Add(new CategoryAssociationModel { CatalogName = units[0], CategoryName = units[1] });
            }

            // Determine if sellable-item should be associated to catalog directly
            foreach (var catalogAssoication in data.CatalogAssociationList)
            {
                if (!data.CategoryAssociationList.Any(c => c.CatalogName.Equals(catalogAssoication.Name)))
                {
                    catalogAssoication.IsParent = true;
                }
            }

            item.Policies.Add(data);
            transientDataList.Add(data);
        }

        private async Task TransformCatalog(CommerceContext commerceContext, List<SellableItem> importItems)
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
                var transientData = item.GetPolicy<TransientImportSellableItemDataPolicy>();

                if (transientData.CatalogAssociationList == null || transientData.CatalogAssociationList.Count().Equals(0))
                    throw new Exception($"{item.Name}: needs to have at least one definied catalog");

                var catalogList = new List<string>();
                var parentCatalogList = new List<string>();
                foreach (var catalogAssociation in transientData.CatalogAssociationList)
                {
                    allCatalogsDictionary.TryGetValue(catalogAssociation.Name, out Sitecore.Commerce.Plugin.Catalog.Catalog catalog);
                    if (catalog != null)
                    {
                        catalogList.Add(catalog.SitecoreId);
                        item.GetComponent<CatalogsComponent>().ChildComponents.Add(new CatalogComponent { Name = catalogAssociation.Name });

                        if (catalogAssociation.IsParent)
                        {
                            transientData.ParentAssociationsToCreateList.Add(new CatalogItemParentAssociationModel(item.Id, catalog.Id, catalog));
                            parentCatalogList.Add(catalog.SitecoreId);
                        }
                    }
                    else
                    {
                        commerceContext.Logger.LogWarning($"Warning, Product with id {item.ProductId} attempting import into catalog {catalogAssociation.Name} which doesn't exist.");
                    }
                }

                item.CatalogToEntityList = catalogList.Any() ? string.Join("|", catalogList) : null;
                item.ParentCatalogList = parentCatalogList.Any() ? string.Join("|", parentCatalogList) : null;
            }
        }

        private async Task TransformCategory(CommerceContext commerceContext, List<TransientImportSellableItemDataPolicy> transientDataList, List<SellableItem> importItems)
        {
            var catalogNameList = transientDataList.SelectMany(d => d.CatalogAssociationList).Select(a => a.Name).Distinct();
            var catalogContextList = await Command<GetCatalogContextCommand>().Process(commerceContext, catalogNameList);

            foreach (var item in importItems)
            {
                var transientData = item.GetPolicy<TransientImportSellableItemDataPolicy>();

                var itemsCategoryList = new List<string>();
                foreach (var categoryAssociation in transientData.CategoryAssociationList)
                {
                    var catalogContext = catalogContextList.FirstOrDefault(c => c.CatalogName.Equals(categoryAssociation.CatalogName));
                    if (catalogContext == null) throw new Exception($"Error, catalog not found {categoryAssociation.CatalogName}. This should not happen.");

                    // Find category
                    catalogContext.CategoriesByName.TryGetValue(categoryAssociation.CategoryName, out Category category);
                    if (category != null)
                    {
                        // Found category
                        itemsCategoryList.Add(category.SitecoreId);
                        transientData.ParentAssociationsToCreateList.Add(new CatalogItemParentAssociationModel(item.Id, categoryAssociation.CatalogName.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>(), category));
                    }
                    else
                    {
                        commerceContext.Logger.LogWarning($"Warning, Product with id {item.ProductId} attempting import into category {categoryAssociation.CategoryName} which doesn't exist.");
                    }
                }

                item.ParentCategoryList = itemsCategoryList.Any() ? string.Join("|", itemsCategoryList) : null;
            }
        }

        private async Task TransformImages(CommerceContext commerceContext, List<TransientImportSellableItemDataPolicy> transientDataList, List<SellableItem> importItems)
        {
            var listOfImageNames = transientDataList.SelectMany(d => d.ImageNameList).Distinct().ToList();
            listOfImageNames = listOfImageNames.Select(imageName => System.IO.Path.GetFileNameWithoutExtension(imageName)).ToList();
            var imageNameDictionary = await Command<GetMediaItemsCommand>().Process(commerceContext, listOfImageNames);

            if (imageNameDictionary.Count().Equals(0))
            {
                commerceContext.Logger.LogWarning($"{nameof(TransformImportToSellableItemsCommand)}-Warning no media found in Sitecore.");
                return;
            }

            foreach (var item in importItems)
            {
                var transientData = item.GetPolicy<TransientImportSellableItemDataPolicy>();

                if (transientData.ImageNameList == null || transientData.ImageNameList.Count().Equals(0))
                    continue;

                var imageComponent = new ImagesComponent();
                foreach (var imageName in transientData.ImageNameList)
                {
                    imageNameDictionary.TryGetValue(System.IO.Path.GetFileNameWithoutExtension(imageName), out string imageId);
                    if (!string.IsNullOrEmpty(imageId)) imageComponent.Images.Add(imageId);
                }

                item.Components.Add(imageComponent);
            }
        }
    }
}
