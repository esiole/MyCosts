namespace MyCosts.Application.Features.Products.GetProducts;

public sealed record ProductSummary(Guid Id, string Name, Guid CategoryId, string CategoryName);
