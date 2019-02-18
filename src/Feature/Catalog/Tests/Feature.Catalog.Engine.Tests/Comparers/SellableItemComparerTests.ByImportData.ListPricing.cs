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
            public class ListPricing
            {
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ListPricingTest_01_PoliciesA_IsNull(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
                    var ItemB = ItemA.Clone();
                    
                    ItemA.Policies = null;
                    
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
                public void SellableItemCompare_ByImportData_ListPricingTest_02_PoliciesB_IsNull(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
                    var ItemB = ItemA.Clone();
                    ItemB.Policies = null;

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
                public void SellableItemCompare_ByImportData_ListPricingTest_03_PoliciesBoth_IsNull(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
                    var ItemB = ItemA.Clone();
                    ItemA.Policies = null;
                    ItemB.Policies = null;

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
                public void SellableItemCompare_ByImportData_ListPricingTest_04_DifferentCount(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
                    var ItemB = ItemA.Clone();
                    var policy = ItemA.GetPolicy<ListPricingPolicy>();
                    policy.RemovePrice(policy.Prices.Cast<Money>().First());

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().NotBe(ItemB.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count());
                    result.Should().BeFalse();
                }

                // Aimed at testing\preventing these types of lists being equal
                // collection1 = {1, 2, 3, 3, 4}
                // collection2 = {1, 2, 2, 3, 4}
                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ListPricingTest_05_SameCount_EachListWithDifferentDuplicates(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
                    var ItemB = ItemA.Clone();
                    var initialPriceCount = ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count();
                    var policyA = ItemA.GetPolicy<ListPricingPolicy>();
                    policyA.AddPrice(policyA.Prices.Cast<Money>().ElementAt(0).Clone());
                    var policyB = ItemA.GetPolicy<ListPricingPolicy>();
                    policyB.AddPrice(policyB.Prices.Cast<Money>().ElementAt(2).Clone());

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().Be(ItemB.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count());
                    // Turns out we can't add duplicates. If this behaviour every changes, you will need to revist.
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().Be(initialPriceCount);
                    result.Should().BeTrue();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ListPricingTest_06_SameCount_OneWithDifferentCurrency(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
                    var ItemB = ItemA.Clone();
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().ElementAt(0).CurrencyCode += "1";

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().BeGreaterThan(0);
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().Be(ItemB.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count());
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ListPricingTest_07_SameCount_OneWithDifferentValue(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
                    var ItemB = ItemA.Clone();
                    ItemB.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().ElementAt(2).Amount += 1;

                    /**********************************************
                     * Act
                     **********************************************/
                    bool result = false;
                    Action executeAction = () => result = comparer.Equals(ItemA, ItemB);

                    /**********************************************
                     * Assert
                     **********************************************/
                    executeAction.Should().NotThrow<Exception>();
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().BeGreaterThan(0);
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().Be(ItemB.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count());
                    result.Should().BeFalse();
                }

                [Theory, AutoNSubstituteData]
                public void SellableItemCompare_ByImportData_ListPricingTest_08_AreEqual(
                    SellableItem ItemA,
                    List<Money> PricesA)
                {
                    /**********************************************
                     * Arrange
                     **********************************************/
                    var comparer = new ImportSellableItemComparer(SellableItemComparerConfiguration.ByImportData);
                    ItemA.GetPolicy<ListPricingPolicy>().AddPrices(PricesA);
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
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().BeGreaterThan(0);
                    ItemA.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count().Should().Be(ItemB.GetPolicy<ListPricingPolicy>().Prices.Cast<Money>().Count());
                    result.Should().BeTrue();
                }
            }
        }
    }
}
