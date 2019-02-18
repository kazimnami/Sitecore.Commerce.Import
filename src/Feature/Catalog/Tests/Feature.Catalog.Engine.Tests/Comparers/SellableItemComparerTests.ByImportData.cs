using Feature.Catalog.Engine.Tests.Utilities;
using FluentAssertions;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using Xunit;

namespace Feature.Catalog.Engine.Tests
{
    public partial class SellableItemComparerTests
    {
        public partial class ByImportData
        {
            [Theory, AutoNSubstituteData]
            public void SellableItemCompare_ByImportData_01_ItemA_IsNull(
                SellableItem ItemB)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);

                /**********************************************
                 * Act
                 **********************************************/
                bool result = false;
                Action executeAction = () => result = comparer.Equals(null, ItemB);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                result.Should().BeFalse();
            }

            [Theory, AutoNSubstituteData]
            public void SellableItemCompare_ByImportData_02_ItemB_IsNull(
                SellableItem ItemA)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);

                /**********************************************
                 * Act
                 **********************************************/
                bool result = false;
                Action executeAction = () => result = comparer.Equals(ItemA, null);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                result.Should().BeFalse();
            }

            [Fact]
            public void SellableItemCompare_ByImportData_03_Both_IsNull()
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);

                /**********************************************
                 * Act
                 **********************************************/
                bool result = false;
                Action executeAction = () => result = comparer.Equals(null, null);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                result.Should().BeFalse();
            }

            [Theory, AutoNSubstituteData]
            public void SellableItemCompare_ByImportData_04_ByImportData_IdsDifferent(
                SellableItem ItemA)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                var ItemB = ItemA.Clone();
                ItemB.ProductId = ItemB.ProductId + "1";

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
            public void SellableItemCompare_ByImportData_05_IdsSame_OtherFieldsSame(
                SellableItem ItemA)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
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
                result.Should().BeTrue();
            }

            [Theory, AutoNSubstituteData]
            public void SellableItemCompare_ByImportData_06_IdsSame_OtherFieldsDifferent(
                SellableItem ItemA)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                var ItemB = ItemA.Clone();
                ItemB.Brand = null;

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
        }
    }
}
