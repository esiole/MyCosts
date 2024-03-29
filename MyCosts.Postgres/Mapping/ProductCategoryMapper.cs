﻿using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Mapping;

internal class ProductCategoryMapper(IEntityMapper<ProductEntity, Product> productMapper) 
    : IEntityMapper<ProductCategoryEntity, ProductCategory>
{
    public ProductCategoryEntity MapToEntity(ProductCategory domainModel) => new()
    {
        Id = domainModel.Id,
        Name = domainModel.Name,
        UserId = domainModel.UserId,
        Products = domainModel.Products.Select(productMapper.MapToEntity).ToList(),
    };

    public ProductCategory MapToDomainModel(ProductCategoryEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        UserId = entity.UserId,
        Products = entity.Products.Select(productMapper.MapToDomainModel).ToList(),
    };
}