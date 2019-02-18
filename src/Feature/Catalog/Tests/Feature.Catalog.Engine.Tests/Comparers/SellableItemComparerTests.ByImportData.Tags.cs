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
            public class Tags
            {
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_TagsTest_01_TagsA_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemA.Tags = null;

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
                public void SellableItemCompare_ByImportData_TagsTest_02_TagsB_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemB.Tags = null;

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
                public void SellableItemCompare_ByImportData_TagsTest_03_Both_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemA.Tags = null;
                    ItemB.Tags = null;

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
                public void SellableItemCompare_ByImportData_TagsTest_04_DifferentCount(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemA.Tags.RemoveAt(0);

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.Tags.Count.Should().NotBe(ItemB.Tags.Count);
                    result.Should().BeFalse();
                }

                // Aimed at testing\preventing these types of lists being equal
                // collection1 = {1, 2, 3, 3, 4}
                // collection2 = {1, 2, 2, 3, 4}
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_TagsTest_05_SameCount_EachListWithDifferentDuplicates(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemA.Tags.Add(ItemA.Tags[0].Clone());
                    ItemB.Tags.Add(ItemB.Tags[2].Clone());

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.Tags.Count.Should().Be(ItemB.Tags.Count);
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_TagsTest_06_AreEqual(
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
                    ItemA.Tags.Count.Should().BeGreaterThan(0);
                    ItemA.Tags.Count.Should().Be(ItemB.Tags.Count);
                    result.Should().BeTrue();
                }
            }
        }
    }
}
