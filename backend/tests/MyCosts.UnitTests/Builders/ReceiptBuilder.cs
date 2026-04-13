using MyCosts.Domain.Receipts;

namespace MyCosts.UnitTests.Builders;

public sealed class ReceiptBuilder
{
    private Guid _id = Guid.CreateVersion7();
    private Guid _userId = Guid.CreateVersion7();
    private DateOnly _purchaseDate = new(2026, 4, 13);
    private DateTimeOffset _createdAt = new(2026, 4, 13, 20, 48, 0, TimeSpan.Zero);
    private string _shop = "Amazon";
    private string _currencyCode = "USD";
    private IReadOnlyCollection<ReceiptLine> _lines = [];

    public ReceiptBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ReceiptBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public ReceiptBuilder WithPurchaseDate(DateOnly date)
    {
        _purchaseDate = date;
        return this;
    }

    public ReceiptBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ReceiptBuilder WithShop(string shop)
    {
        _shop = shop;
        return this;
    }

    public ReceiptBuilder WithCurrency(string currencyCode)
    {
        _currencyCode = currencyCode;
        return this;
    }

    public ReceiptBuilder WithLines(params IReadOnlyCollection<ReceiptLine> lines)
    {
        _lines = lines;
        return this;
    }

    public Receipt Build() => new(_id, _userId, _purchaseDate, _createdAt, _shop, _currencyCode, _lines);
}
