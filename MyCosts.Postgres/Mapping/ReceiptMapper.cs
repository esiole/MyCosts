using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;

namespace MyCosts.Postgres.Mapping;

public static class ReceiptMapper
{
    public static ReceiptEntity ToEntity(this Receipt domainModel) => new()
    {
        Id = domainModel.Id,
        Date = domainModel.Date,
        PlaceName = domainModel.PlaceName,
        UserId = domainModel.UserId,
        Costs = domainModel.Costs.Select(c => new CostEntity
        {
            Id = c.Id,
            Amount = c.Amount,
            Count = c.Count,
            Weight = c.Weight,
            ProductId = c.ProductId,
            ReceiptId = domainModel.Id,
        }).ToArray(),
    };

    public static Receipt ToDomainModel(this ReceiptEntity entity) => new()
    {
        Id = entity.Id,
        Date = entity.Date,
        PlaceName = entity.PlaceName,
        UserId = entity.UserId,
        Costs = entity.Costs.Select(c => new Cost
        {
            Id = c.Id,
            Amount = c.Amount,
            Count = c.Count,
            Weight = c.Weight,
            ProductId = c.ProductId,
        }).ToArray(),
    };
}