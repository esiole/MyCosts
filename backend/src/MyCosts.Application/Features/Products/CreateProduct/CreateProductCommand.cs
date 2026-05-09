using System.ComponentModel.DataAnnotations;
using MyCosts.Domain.Products;

namespace MyCosts.Application.Features.Products.CreateProduct;

public sealed record CreateProductCommand(
    Guid UserId,
    Guid CategoryId,
    [property: Required, MinLength(ProductConstraints.NameMinLength), MaxLength(ProductConstraints.NameMaxLength)]
    string Name);
