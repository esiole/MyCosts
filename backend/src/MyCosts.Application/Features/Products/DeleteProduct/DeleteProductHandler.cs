using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler(AppDbContext db)
{
    public async Task<Result> HandleAsync(DeleteProductCommand cmd, CancellationToken ct)
    {
        var affected = await db.Products
            .Where(p => p.Id == cmd.ProductId &&
                        db.ProductCategories.Any(c => c.Id == p.CategoryId && c.UserId == cmd.UserId))
            .ExecuteDeleteAsync(ct);

        return affected == 0 ? Result.Fail(ErrorKind.NotFound, "Product not found.") : Result.Ok();
    }
}
