namespace MyCosts.Api.Models.Cost;

public class CostEditModel
{
    public decimal Amount { get; init; }
    public int Count { get; set; }
    public double? WeightInKg { get; set; }
    public required int ProductId { get; set; }
}