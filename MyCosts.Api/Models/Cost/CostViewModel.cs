namespace MyCosts.Api.Models.Cost;

public class CostViewModel
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
    public int Count { get; set; }
    public double? WeightInKg { get; set; }
    public required int ProductId { get; set; }
}