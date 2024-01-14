using MyCosts.Postgres.Entities.Abstractions;

namespace MyCosts.Postgres.Entities;

public class CostEntity : IPostgresEntity
{
    public int Id { get; set; }

    public required decimal Amount { get; set; }
    public int Count { get; set; }
    public double? Weight { get; set; }

    public required int ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public int ReceiptId { get; set; }
    public ReceiptEntity? Receipt { get; set; }
}