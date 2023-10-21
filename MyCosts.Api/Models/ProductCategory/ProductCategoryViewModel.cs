using MyCosts.Api.Models.Product;

namespace MyCosts.Api.Models.ProductCategory;

public class ProductCategoryViewModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required ProductViewModel[] Products { get; init; }
}