using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.ProductCategories.UpdateProductCategory;

public sealed class UpdateProductCategoryHandler(AppDbContext db)
{
    public async Task<Result> HandleAsync(UpdateProductCategoryCommand cmd, CancellationToken ct)
    {
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(cmd, new ValidationContext(cmd), validationResults, true))
        {
            return Result.ValidationFail(validationResults);
        }

        var name = cmd.Name.Trim();
        if (name.Length == 0)
        {
            return Result.ValidationFail([
                new ValidationResult(
                    "Name must not be empty or whitespace.",
                    [nameof(cmd.Name)])
            ]);
        }

        if (await db.ProductCategories.AnyAsync(c =>
                    c.UserId == cmd.UserId && c.Name == name && c.Id != cmd.CategoryId,
                ct))
        {
            return Result.Fail(ErrorKind.Conflict, "A category with this name already exists.");
        }

        try
        {
            var affected = await db.ProductCategories
                .Where(c => c.Id == cmd.CategoryId && c.UserId == cmd.UserId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.Name, name), ct);

            return affected == 0 ? Result.Fail(ErrorKind.NotFound, "Category not found.") : Result.Ok();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation)
        {
            return Result.Fail(ErrorKind.Conflict, "A category with this name already exists.");
        }
    }
}
