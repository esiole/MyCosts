using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Mapping;

internal class ReceiptMapper : IEntityMapper<ReceiptEntity, Receipt>
{
    public ReceiptEntity MapToEntity(Receipt domainModel) => new()
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

    public Receipt MapToDomainModel(ReceiptEntity entity) => new()
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