using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        // List Price
        private const int ListPriceCurrencyCodeIndex = 0;
        private const int ListPriceAmountIndex = 1;

        public TransformImportToSellableItemsCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<IEnumerable<SellableItem>> Process(CommerceContext commerceContext, IEnumerable<string[]> importRawLines)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var importItems = new List<SellableItem>();
                var transientDataList = new List<TransientImportDataPolicy>();
                foreach (var rawFields in importRawLines)
                {
                    var item = new SellableItem();
                    TransformCore(commerceContext, rawFields, item);
                    TransformListPrice(rawFields, item);
                    TransformProductExtension(rawFields, item);
                    TransformTransientData(rawFields, item, transientDataList);
                    importItems.Add(item);
                }

                await TransformCatalog(commerceContext, transientDataList, importItems);
                await TransformCategory(commerceContext, transientDataList, importItems);
                await TransformImages(commerceContext, transientDataList, importItems);
                RemoveTransientData(importItems);

                return importItems;
            }
        }

        private void TransformCore(CommerceContext commerceContext, string[] rawFields, SellableItem item)
        {
            item.ProductId = rawFields[ProductIdIndex];
            item.Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{item.ProductId}";
            item.FriendlyId = item.ProductId;
            item.Name = rawFields[ProductNameIndex];
            /// Be sure not to include SitecoreId in <see cref="ProductExtensionComponentComparer"/> 
            item.SitecoreId = GuidUtils.GetDeterministicGuidString(item.Id);
            item.DisplayName = rawFields[DisplayNameIndex];
            item.Description = rawFields[DescriptionIndex];
            item.Brand = rawFields[BrandIndex];
            item.Manufacturer = rawFields[ManufacturerIndex];
            item.TypeOfGood = rawFields[TypeOfGoodIndex];
            var tags = string.IsNullOrEmpty(rawFields[TagsIndex])? null : rawFields[TagsIndex].Split('|');
            item.Tags = tags == null ? new List<Tag>() : tags.Select(x => new Tag(x)).ToList();

            var component = item.GetComponent<ListMembershipsComponent>();
            component.Memberships.Add(string.Format("{0}", CommerceEntity.ListName<SellableItem>()));
            component.Memberships.Add(commerceContext.GetPolicy<KnownCatalogListsPolicy>().CatalogItems);
        }

        private void TransformListPrice(string[] rawFields, SellableItem item)
        {
            var listPrices = rawFields[ListPriceIndex].Split('|');
            var priceList = new List<Money>();
            foreach (var listPrice in listPrices)
            {
                var priceData = listPrice.Split('-');
                priceList.Add(new Money
                {
                    CurrencyCode = priceData[ListPriceCurrencyCodeIndex],
                    Amount = decimal.Parse(priceData[ListPriceAmountIndex])
                });
            }

            item.Policies.Add(new ListPricingPolicy(priceList));
        }

        private void TransformProductExtension(string[] rawFields, SellableItem item)
        {
            item.Components.Add(new ProductExtensionComponent
            {
                Style = rawFields[StyleIndex],
                FuelType = rawFields[FuelTypeIndex],
                NaturalGasConversionAvailable = rawFields[NaturalGasConversionAvailableIndex],
                DimensionsHeightHoodOpen = rawFields[DimensionsHeightHoodOpenIndex],
                DimensionsHeightHoodClosed = rawFields[DimensionsHeightHoodClosedIndex],
                DimensionsWidth = rawFields[DimensionsWidthIndex],
                DimensionsDepth = rawFields[DimensionsDepthIndex]
            });
        }

        private void TransformTransientData(string[] rawFields, SellableItem item, List<TransientImportDataPolicy> transientDataList)
        {
            var data = new TransientImportDataPolicy
            {
                ParentCatalogNameList = rawFields[CatalogNameIndex].Split('|').ToList(),
                ParentCategoryNameList = rawFields[CategoryNameIndex].Split('|').ToList(),
                ImageNameList = rawFields[ImagesIndex].Split('|')
            };
            item.Policies.Add(data);
            transientDataList.Add(data);
        }

        private async Task TransformCatalog(CommerceContext commerceContext, List<TransientImportDataPolicy> transientDataList, List<SellableItem> importItems)
        {
            var listOfCatalogNames = transientDataList.SelectMany(d => d.ParentCatalogNameList).ToList().Distinct();
            var allCatalogs = await Command<GetCatalogsCommand>().Process(commerceContext);
            var allCatalogsDictionary = allCatalogs.ToDictionary(c => c.Name);

            foreach (var item in importItems)
            {
                var transientData = item.GetPolicy<TransientImportDataPolicy>();

                if (transientData.ParentCatalogNameList == null || transientData.ParentCatalogNameList.Count().Equals(0))
                    throw new Exception($"{item.Name}: needs to have at least one definied catalog");

                var itemsCatalogList = new List<string>();
                var catalogsComponent = item.GetComponent<CatalogsComponent>();
                foreach (var catalogName in transientData.ParentCatalogNameList)
                {
                    allCatalogsDictionary.TryGetValue(catalogName, out Sitecore.Commerce.Plugin.Catalog.Catalog catalog);
                    if (catalog != null)
                    {
                        itemsCatalogList.Add(catalog.SitecoreId);
                        catalogsComponent.ChildComponents.Add(new CatalogComponent { Name = catalogName });
                    }
                    else
                    {
                        commerceContext.Logger.LogWarning($"Warning, Product with id {item.ProductId} attempting import into catalog {catalogName} which doesn't exist.");
                    }
                }

                item.ParentCatalogList = string.Join("|", itemsCatalogList);
                item.CatalogToEntityList = item.ParentCatalogList;
            }
        }

        private async Task TransformCategory(CommerceContext commerceContext, List<TransientImportDataPolicy> transientDataList, List<SellableItem> importItems)
        {
            // This method would need to be enhanced if you need to support the same categories existing in multiple catalogs
            // You would basically need to introduce a catalog to category lookup
            var listOfCatalogNames = transientDataList.SelectMany(d => d.ParentCatalogNameList).ToList().Distinct();
            if (listOfCatalogNames.Count() > 1) throw new Exception($"{nameof(TransformCategory)} does not currently support multiple catalogs");

            var listOfCategoryNames = transientDataList.SelectMany(d => d.ParentCategoryNameList).ToList().Distinct();
            var allCategories = await Command<GetCategoriesCommand>().Process(commerceContext, listOfCatalogNames.First());
            var allCategoriesDictionary = allCategories.ToDictionary(c => c.Name);

            foreach (var item in importItems)
            {
                var transientData = item.GetPolicy<TransientImportDataPolicy>();

                if (transientData.ParentCategoryNameList == null || transientData.ParentCategoryNameList.Count().Equals(0))
                    throw new Exception($"{item.Name}: needs to have at least one definied Category");

                var itemsCategoryList = new List<string>();
                foreach (var CategoryName in transientData.ParentCategoryNameList)
                {
                    allCategoriesDictionary.TryGetValue(CategoryName, out Category category);
                    if (category != null)
                    {
                        itemsCategoryList.Add(category.SitecoreId);
                    }
                    else
                    {
                        commerceContext.Logger.LogWarning($"Warning, Product with id {item.ProductId} attempting import into category {CategoryName} which doesn't exist.");
                    }
                }

                item.ParentCategoryList = string.Join("|", itemsCategoryList);
            }
        }

        private async Task TransformImages(CommerceContext commerceContext, List<TransientImportDataPolicy> transientDataList, List<SellableItem> importItems)
        {
            var listOfImageNames = transientDataList.SelectMany(d => d.ImageNameList).ToList().Distinct();
            var imageNameDictionary = await Command<GetMediaItemsCommand>().Process(commerceContext, listOfImageNames);

            if (imageNameDictionary.Count().Equals(0))
            {
                commerceContext.Logger.LogWarning($"{nameof(TransformImportToSellableItemsCommand)}-Warning no media found in Sitecore.");
                return;
            }

            foreach (var item in importItems)
            {
                var transientData = item.GetPolicy<TransientImportDataPolicy>();

                if (transientData.ImageNameList == null || transientData.ImageNameList.Count().Equals(0))
                    continue;

                var imageComponent = new ImagesComponent();
                foreach(var imageName in transientData.ImageNameList)
                {
                    imageNameDictionary.TryGetValue(imageName, out string imageId);
                    if (!string.IsNullOrEmpty(imageId)) imageComponent.Images.Add(imageId);
                }

                item.Components.Add(imageComponent);
            }
        }

        private void RemoveTransientData(List<SellableItem> importItems)
        {
            foreach (var item in importItems)
            {
                if (item.HasPolicy<TransientImportDataPolicy>())
                    item.RemovePolicy(typeof(TransientImportDataPolicy));
            }
        }
    }
}
