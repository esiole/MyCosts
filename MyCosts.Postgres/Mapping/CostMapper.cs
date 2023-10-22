﻿using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;

namespace MyCosts.Postgres.Mapping;

public static class CostMapper
{
    public static CostEntity ToEntity(this Cost domainModel) => new()
    {
        Id = domainModel.Id,
        Amount = domainModel.Amount,
        Count = domainModel.Count,
        WeightInKg = domainModel.WeightInKg,
        ProductId = domainModel.ProductId,
    };

    public static Cost ToDomainModel(this CostEntity entity) => new()
    {
        Id = entity.Id,
        Amount = entity.Amount,
        Count = entity.Count,
        WeightInKg = entity.WeightInKg,
        ProductId = entity.ProductId,
    };
}