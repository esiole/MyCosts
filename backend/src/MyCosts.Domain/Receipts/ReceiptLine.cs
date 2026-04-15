namespace MyCosts.Domain.Receipts;

public sealed class ReceiptLine
{
    public Guid Id { get; init; }
    public Guid ReceiptId { get; init; }
    public Guid ProductId { get; init; }
    public LinePricing LinePricing { get; init; }

    public decimal TotalAmount => LinePricing.TotalAmount;

    // Used by EF Core
    private ReceiptLine()
    {
        LinePricing = null!;
    }

    public ReceiptLine(Guid id, Guid receiptId, Guid productId, LinePricing linePricing)
    {
        Id = id;
        ReceiptId = receiptId;
        ProductId = productId;
        LinePricing = linePricing;
    }
}
