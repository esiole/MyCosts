using System.ComponentModel.DataAnnotations;
using MyCosts.Domain.Products;

namespace MyCosts.Application.Features.ProductCategories.CreateProductCategory;

public sealed record CreateProductCategoryCommand(
    Guid UserId,
    [property: Required, MinLength(ProductCategoryConstraints.NameMinLength), MaxLength(ProductCategoryConstraints.NameMaxLength)]
    string Name);
