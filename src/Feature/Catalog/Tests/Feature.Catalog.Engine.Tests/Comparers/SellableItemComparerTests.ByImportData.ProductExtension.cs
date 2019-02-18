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
            public class ProductExtension
            {
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ProductExtensionComponent_01_ItemA_IsNull(
                    SellableItem ItemA,
                    ProductExtensionComponent ProductExtensionComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ProductExtensionComponentA);
                    var ItemB = ItemA.Clone();

                    ItemA.GetComponent<ProductExtensionComponent>().Style = null;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ProductExtensionComponent_02_ItemB_IsNull(
                    SellableItem ItemA,
                    ProductExtensionComponent ProductExtensionComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ProductExtensionComponentA);
                    var ItemB = ItemA.Clone();

                    ItemB.GetComponent<ProductExtensionComponent>().Style = null;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ProductExtensionComponent_03_ItemBoth_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemA.GetComponent<ProductExtensionComponent>().Style = null;
                    ItemB.GetComponent<ProductExtensionComponent>().Style = null;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetComponent<ProductExtensionComponent>().Style.Should().BeNull();
                    result.Should().BeTrue();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ProductExtensionComponent_04_AreEqual(
                    SellableItem ItemA,
                    ProductExtensionComponent ProductExtensionComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ProductExtensionComponentA);
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
                    ItemA.GetComponent<ProductExtensionComponent>().Style.Should().NotBeNullOrWhiteSpace();
                    result.Should().BeTrue();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ProductExtensionComponent_05_AreDifferent(
                    SellableItem ItemA,
                    ProductExtensionComponent ProductExtensionComponentA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.Components.Add(ProductExtensionComponentA);
                    var ItemB = ItemA.Clone();

                    ItemB.GetComponent<ProductExtensionComponent>().Style += "1";

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetComponent<ProductExtensionComponent>().Style.Should().NotBeNullOrWhiteSpace();
                    result.Should().BeFalse();
                }
            }
        }
    }
}
