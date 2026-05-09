using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Domain.Products;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.Products.CreateProduct;

public sealed class CreateProductHandler(AppDbContext db)
{
    public async Task<Result<Guid>> HandleAsync(CreateProductCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.Name))
        {
            return Result<Guid>.Fail(ErrorKind.Validation, "Name is required.");
        }

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(cmd, new ValidationContext(cmd), validationResults, true))
        {
            return Result<Guid>.ValidationFail(validationResults);
        }

        var categoryExists = await db.ProductCategories
            .AnyAsync(c => c.Id == cmd.CategoryId && c.UserId == cmd.UserId, ct);
        if (!categoryExists)
        {
            return Result<Guid>.Fail(ErrorKind.NotFound, "Category not found.");
        }

        var product = new Product(Guid.CreateVersion7(), cmd.CategoryId, cmd.Name);
        db.Products.Add(product);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation)
        {
            return Result<Guid>.Fail(ErrorKind.NotFound, "Category not found.");
        }

        return Result<Guid>.Ok(product.Id);
    }
}
