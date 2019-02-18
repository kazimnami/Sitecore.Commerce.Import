using Feature.Catalog.Engine.Tests.Utilities;
using FluentAssertions;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Feature.Catalog.Engine.Tests
{
    public partial class SellableItemComparerTests
    {
        public partial class ByImportData
        {
            public class Images
            {
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Images_01_ImagesA_IsNull(
                    SellableItem ItemA,
                    ImagesComponent ImagesComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ImagesComponentA);
                    var ItemB = ItemA.Clone();

                    ItemA.GetComponent<ImagesComponent>().Images = null;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemB.GetComponent<ImagesComponent>().Images.Count().Should().BeGreaterThan(0);
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Images_02_ImagesB_IsNull(
                    SellableItem ItemA,
                    ImagesComponent ImagesComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ImagesComponentA);
                    var ItemB = ItemA.Clone();

                    ItemB.GetComponent<ImagesComponent>().Images = null;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetComponent<ImagesComponent>().Images.Count().Should().BeGreaterThan(0);
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Images_03_ImagesBoth_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemA.GetComponent<ImagesComponent>().Images = null;
                    ItemB.GetComponent<ImagesComponent>().Images = null;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetComponent<ImagesComponent>().Images.Should().BeNull();
                    result.Should().BeTrue();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Images_04_DifferentCount(
                    SellableItem ItemA,
                    ImagesComponent ImagesComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ImagesComponentA);
                    var ItemB = ItemA.Clone();

                    ItemA.GetComponent<ImagesComponent>().Images.RemoveAt(1);

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetComponent<ImagesComponent>().Images.Count().Should().NotBe(ItemB.GetComponent<ImagesComponent>().Images.Count());
                    result.Should().BeFalse();
                }

                // Aimed at testing\preventing these types of lists being equal
                // collection1 = {1, 2, 3, 3, 4}
                // collection2 = {1, 2, 2, 3, 4}
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Images_05_SameCount_EachListWithDifferentDuplicates(
                    SellableItem ItemA,
                    ImagesComponent ImagesComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ImagesComponentA);
                    var ItemB = ItemA.Clone();

                    ItemA.GetComponent<ImagesComponent>().Images.Add(ItemA.GetComponent<ImagesComponent>().Images[0].Clone().ToString());
                    ItemB.GetComponent<ImagesComponent>().Images.Add(ItemB.GetComponent<ImagesComponent>().Images[2].Clone().ToString());

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetComponent<ImagesComponent>().Images.Count().Should().Be(ItemB.GetComponent<ImagesComponent>().Images.Count());
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Images_06_AreEqual(
                    SellableItem ItemA,
                    ImagesComponent ImagesComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ImagesComponentA);
                    var ItemB = ItemA.Clone();

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetComponent<ImagesComponent>().Images.Count().Should().BeGreaterThan(0);
                    ItemA.GetComponent<ImagesComponent>().Images.Count().Should().Be(ItemB.GetComponent<ImagesComponent>().Images.Count());
                    result.Should().BeTrue();
                }
            }
        }
    }
}
