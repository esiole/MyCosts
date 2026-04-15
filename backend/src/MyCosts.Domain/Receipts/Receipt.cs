namespace MyCosts.Domain.Receipts;

public sealed class Receipt
{
    private readonly List<ReceiptLine> _lines;

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateOnly PurchaseDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public string Shop { get; init; }
    public string CurrencyCode { get; init; }

    public IReadOnlyList<ReceiptLine> Lines => _lines;

    public Receipt(
        Guid id,
        Guid userId,
        DateOnly purchaseDate,
        DateTimeOffset createdAt,
        string shop,
        string currencyCode,
        IReadOnlyCollection<ReceiptLine> lines)
    {
        Id = id;
        UserId = userId;
        PurchaseDate = purchaseDate;
        CreatedAt = createdAt;
        Shop = shop;
        CurrencyCode = currencyCode;
        _lines = [..lines];
    }

    // Used by EF Core
    private Receipt()
    {
        Shop = null!;
        CurrencyCode = null!;
        _lines = null!;
    }

    public ReceiptLine AddLine(Guid productId, LinePricing linePricing)
    {
        var line = new ReceiptLine(Guid.CreateVersion7(), Id, productId, linePricing);
        _lines.Add(line);
        return line;
    }
}
