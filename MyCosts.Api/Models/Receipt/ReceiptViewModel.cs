using MyCosts.Api.Models.Cost;

namespace MyCosts.Api.Models.Receipt;

public class ReceiptViewModel
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public required string PlaceName { get; set; }
    public required CostModel[] Costs { get; set; }
}