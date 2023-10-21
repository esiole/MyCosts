using MyCosts.Api.Models.ProductCategory;
using MyCosts.Domain.Models;

namespace MyCosts.Api.Mapping;

public static class ProductCategoryMapping
{
    public static ProductCategory ToProductCategory(this ProductCategoryEditModel model, User owner, int id = default) => new()
    {
        Id = id,
        Name = model.Name,
        UserId = owner.Id,
    };

    public static ProductCategoryViewModel ToViewModel(this ProductCategory productCategory) => new()
    {
        Id = productCategory.Id,
        Name = productCategory.Name,
    };
}