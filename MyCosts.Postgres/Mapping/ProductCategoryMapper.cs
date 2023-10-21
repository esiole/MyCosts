using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;

namespace MyCosts.Postgres.Mapping;

public static class ProductCategoryMapper
{
    public static ProductCategoryEntity ToEntity(this ProductCategory domainModel) => new()
    {
        Id = domainModel.Id,
        Name = domainModel.Name,
        UserId = domainModel.UserId,
        Products = domainModel.Products.Select(p => p.ToEntity()).ToList(),
    };

    public static ProductCategory ToDomainModel(this ProductCategoryEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        UserId = entity.UserId,
        Products = entity.Products.Select(p => p.ToDomainModel()).ToList(),
    };
}