using System.ComponentModel.DataAnnotations;
using MyCosts.Domain.Products;

namespace MyCosts.Application.Features.ProductCategories.UpdateProductCategory;

public sealed record UpdateProductCategoryCommand(
    Guid UserId,
    Guid CategoryId,
    [property: Required, MinLength(ProductCategoryConstraints.NameMinLength), MaxLength(ProductCategoryConstraints.NameMaxLength)]
    string Name);
