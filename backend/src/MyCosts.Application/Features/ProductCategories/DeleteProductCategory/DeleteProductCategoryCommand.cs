namespace MyCosts.Application.Features.ProductCategories.DeleteProductCategory;

public sealed record DeleteProductCategoryCommand(Guid UserId, Guid CategoryId);
