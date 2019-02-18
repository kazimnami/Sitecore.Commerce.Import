using Feature.Catalog.Engine.Tests.Utilities;
using FluentAssertions;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Linq;
using Xunit;

namespace Feature.Catalog.Engine.Tests
{
    public partial class SellableItemComparerTests
    {
        public partial class ByImportData
        {
            public class Catalogs
            {
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Catalogs_01_ListA_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";
                    var ItemB = ItemA.Clone();

                    ItemA.ParentCatalogList = null;

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
                public void SellableItemCompare_ByImportData_Catalogs_02_ListB_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";
                    var ItemB = ItemA.Clone();

                    ItemB.ParentCatalogList = null;

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
                public void SellableItemCompare_ByImportData_Catalogs_03_ListBoth_IsNull(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    var ItemB = ItemA.Clone();
                    ItemA.ParentCatalogList = null;
                    ItemB.ParentCatalogList = null;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.ParentCatalogList.Should().BeNull();
                    result.Should().BeTrue();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Catalogs_04_DifferentCount(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";
                    var ItemB = ItemA.Clone();
                    ItemB.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.ParentCatalogList.Split('|').Count().Should().NotBe(ItemB.ParentCatalogList.Split('|').Count());
                    result.Should().BeFalse();
                }

                // Aimed at testing\preventing these types of lists being equal
                // collection1 = {1, 2, 3, 3, 4}
                // collection2 = {1, 2, 2, 3, 4}
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Catalogs_05_SameCount_EachListWithDifferentDuplicates(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|440c1cd5-20c1-e0d1-3319-9766112ed9ca|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";
                    var ItemB = ItemA.Clone();
                    ItemB.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.ParentCatalogList.Split('|').Count().Should().Be(ItemB.ParentCatalogList.Split('|').Count());
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Catalogs_06_AreEqual(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";
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
                    ItemA.ParentCatalogList.Split('|').Count().Should().BeGreaterThan(0);
                    ItemA.ParentCatalogList.Split('|').Count().Should().Be(ItemB.ParentCatalogList.Split('|').Count());
                    result.Should().BeTrue();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_Catalogs_07_SameCountButDifferentValues(
                    SellableItem ItemA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|59ddadc1-9b88-727e-9e14-3f6cf321ae0f";
                    var ItemB = ItemA.Clone();
                    ItemB.ParentCatalogList = "06f5147b-9e24-fa33-20fb-d4b7d8d21392|440c1cd5-20c1-e0d1-3319-9766112ed9ca|f8fd76b4-d5b0-4fa2-b881-2867488e5609";

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.ParentCatalogList.Split('|').Count().Should().BeGreaterThan(0);
                    ItemA.ParentCatalogList.Split('|').Count().Should().Be(ItemB.ParentCatalogList.Split('|').Count());
                    result.Should().BeFalse();
                }
            }
        }
    }
}
