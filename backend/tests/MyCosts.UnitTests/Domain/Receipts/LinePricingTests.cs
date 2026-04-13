using MyCosts.Domain.Receipts;
using Xunit;

namespace MyCosts.UnitTests.Domain.Receipts;

public sealed class LinePricingTests
{
    [Fact]
    public void ByQuantity_SetsKindToQuantity()
    {
        var pricing = LinePricing.ByQuantity(3, 50);

        Assert.Equal(LinePricingKind.Quantity, pricing.Kind);
    }

    [Fact]
    public void ByWeight_SetsKindToWeight()
    {
        var pricing = LinePricing.ByWeight(0.5m, 200);

        Assert.Equal(LinePricingKind.Weight, pricing.Kind);
    }

    [Fact]
    public void TotalAmount_ByQuantity_ReturnsQuantityTimesUnitPrice()
    {
        var pricing = LinePricing.ByQuantity(3, 50);

        Assert.Equal(150, pricing.TotalAmount);
    }

    [Fact]
    public void TotalAmount_ByWeight_ReturnsWeightTimesUnitPrice()
    {
        var pricing = LinePricing.ByWeight(0.5m, 200);

        Assert.Equal(100, pricing.TotalAmount);
    }

    [Fact]
    public void TotalAmount_WithFractionalValues_IsExact()
    {
        var pricing = LinePricing.ByQuantity(3, 33.33m);

        Assert.Equal(99.99m, pricing.TotalAmount);
    }
}
