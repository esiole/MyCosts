namespace MyCosts.Domain.Receipts;

public sealed record LinePricing(LinePricingKind Kind, decimal Quantity, decimal UnitPrice)
{
    public decimal TotalAmount => Quantity * UnitPrice;

    public static LinePricing ByQuantity(decimal quantity, decimal unitPrice) =>
        new(LinePricingKind.Quantity, quantity, unitPrice);

    public static LinePricing ByWeight(decimal weightKg, decimal unitPrice) =>
        new(LinePricingKind.Weight, weightKg, unitPrice);
}
