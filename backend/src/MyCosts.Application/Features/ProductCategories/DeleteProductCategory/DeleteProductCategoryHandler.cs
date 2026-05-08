using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.ProductCategories.DeleteProductCategory;

public sealed class DeleteProductCategoryHandler(AppDbContext db)
{
    public async Task<Result> HandleAsync(DeleteProductCategoryCommand cmd, CancellationToken ct)
    {
        var affected = await db.ProductCategories
            .Where(c => c.Id == cmd.CategoryId && c.UserId == cmd.UserId)
            .ExecuteDeleteAsync(ct);

        return affected == 0 ? Result.Fail(ErrorKind.NotFound, "Category not found.") : Result.Ok();
    }
}
