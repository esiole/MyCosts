namespace MyCosts.Application.Features.Products.GetProducts;

public sealed record GetProductsQuery(
    Guid UserId,
    string? Search = null,
    string? Cursor = null,
    int? Limit = null);
