using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Mapping;

internal class ProductMapper : IEntityMapper<ProductEntity, Product>
{
    public ProductEntity MapToEntity(Product domainModel) => new()
    {
        Id = domainModel.Id,
        Name = domainModel.Name,
        CategoryId = domainModel.CategoryId,
    };

    public Product MapToDomainModel(ProductEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        CategoryId = entity.CategoryId,
    };
}