namespace MyCosts.Application.Features.ProductCategories.GetProductCategories;

public sealed record GetProductCategoriesQuery(
    Guid UserId,
    string? Search = null,
    string? Cursor = null,
    int? Limit = null);
