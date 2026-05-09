using System.ComponentModel.DataAnnotations;
using MyCosts.Domain.Products;

namespace MyCosts.Application.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid UserId,
    Guid ProductId,
    Guid CategoryId,
    [property: Required, MinLength(ProductConstraints.NameMinLength), MaxLength(ProductConstraints.NameMaxLength)]
    string Name);
