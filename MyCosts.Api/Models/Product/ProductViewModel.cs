namespace MyCosts.Api.Models.Product;

public class ProductViewModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required int CategoryId { get; init; }
}