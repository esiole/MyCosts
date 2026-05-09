namespace MyCosts.Application.Features.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid UserId, Guid ProductId);
