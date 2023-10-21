namespace MyCosts.Api.Models.Product;

public class ProductEditModel
{
    public required string Name { get; init; }
    public required int CategoryId { get; init; }
}