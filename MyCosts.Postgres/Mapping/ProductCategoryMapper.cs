using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Mapping;

internal class ProductCategoryMapper : IEntityMapper<ProductCategoryEntity, ProductCategory>
{
    private readonly IEntityMapper<ProductEntity, Product> _productMapper;

    public ProductCategoryMapper(IEntityMapper<ProductEntity, Product> productMapper)
    {
        _productMapper = productMapper;
    }

    public ProductCategoryEntity MapToEntity(ProductCategory domainModel) => new()
    {
        Id = domainModel.Id,
        Name = domainModel.Name,
        UserId = domainModel.UserId,
        Products = domainModel.Products.Select(_productMapper.MapToEntity).ToList(),
    };

    public ProductCategory MapToDomainModel(ProductCategoryEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        UserId = entity.UserId,
        Products = entity.Products.Select(_productMapper.MapToDomainModel).ToList(),
    };
}