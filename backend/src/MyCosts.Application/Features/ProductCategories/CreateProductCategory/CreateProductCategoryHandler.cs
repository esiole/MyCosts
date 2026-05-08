using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Domain.Products;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.ProductCategories.CreateProductCategory;

public sealed class CreateProductCategoryHandler(AppDbContext db)
{
    public async Task<Result<Guid>> HandleAsync(CreateProductCategoryCommand cmd, CancellationToken ct)
    {
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(cmd, new ValidationContext(cmd), validationResults, true))
        {
            return Result<Guid>.ValidationFail(validationResults);
        }

        var name = cmd.Name.Trim();
        if (name.Length == 0)
        {
            return Result<Guid>.ValidationFail([
                new ValidationResult(
                    "Name must not be empty or whitespace.",
                    [nameof(cmd.Name)])
            ]);
        }

        if (await db.ProductCategories.AnyAsync(c => c.UserId == cmd.UserId && c.Name == name, ct))
        {
            return Result<Guid>.Fail(ErrorKind.Conflict, "A category with this name already exists.");
        }

        var category = new ProductCategory(Guid.CreateVersion7(), cmd.UserId, name);
        db.ProductCategories.Add(category);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation)
        {
            return Result<Guid>.Fail(ErrorKind.Conflict, "A category with this name already exists.");
        }

        return Result<Guid>.Ok(category.Id);
    }
}
