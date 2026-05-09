using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductHandler(AppDbContext db)
{
    public async Task<Result> HandleAsync(UpdateProductCommand cmd, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cmd.Name))
        {
            return Result.ValidationFail([new ValidationResult("Name is required.", [nameof(cmd.Name)])]);
        }

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(cmd, new ValidationContext(cmd), validationResults, true))
        {
            return Result.ValidationFail(validationResults);
        }

        var targetCategoryExists = await db.ProductCategories
            .AnyAsync(c => c.Id == cmd.CategoryId && c.UserId == cmd.UserId, ct);
        if (!targetCategoryExists)
        {
            return Result.Fail(ErrorKind.NotFound, "Category not found.");
        }

        try
        {
            var affected = await db.Products
                .Where(p => p.Id == cmd.ProductId &&
                            db.ProductCategories.Any(c => c.Id == p.CategoryId && c.UserId == cmd.UserId))
                .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Name, cmd.Name)
                        .SetProperty(p => p.CategoryId, cmd.CategoryId),
                    ct);

            return affected == 0 ? Result.Fail(ErrorKind.NotFound, "Product not found.") : Result.Ok();
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation)
        {
            return Result.Fail(ErrorKind.NotFound, "Category not found.");
        }
    }
}
