using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;

namespace MyCosts.Postgres.Mapping;

public static class ProductMapper
{
    public static ProductEntity ToEntity(this Product domainModel) => new()
    {
        Id = domainModel.Id,
        Name = domainModel.Name,
        CategoryId = domainModel.CategoryId,
    };

    public static Product ToDomainModel(this ProductEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        CategoryId = entity.CategoryId,
    };
}