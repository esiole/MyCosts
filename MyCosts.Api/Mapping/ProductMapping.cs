using MyCosts.Api.Models.Product;
using MyCosts.Domain.Models;

namespace MyCosts.Api.Mapping;

public static class ProductMapping
{
    public static Product ToProduct(this ProductEditModel model, int id = default) => new()
    {
        Id = id,
        Name = model.Name,
        CategoryId = model.CategoryId,
    };

    public static ProductViewModel ToViewModel(this Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        CategoryId = product.CategoryId,
    };
}