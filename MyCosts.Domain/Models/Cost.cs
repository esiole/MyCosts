namespace MyCosts.Domain.Models;

public class Cost
{
    public required int Id { get; set; }

    public required decimal Amount { get; set; }
    public int Count { get; set; } = 1;
    public double? WeightInKg { get; set; }

    public required int ProductId { get; set; }
    public Product? Product { get; set; }
}