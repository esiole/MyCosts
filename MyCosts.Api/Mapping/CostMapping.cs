using MyCosts.Api.Models.Cost;
using MyCosts.Domain.Models;

namespace MyCosts.Api.Mapping;

public static class CostMapping
{
    public static Cost ToCost(this CostEditModel model, int id = default) => new()
    {
        Id = id,
        Amount = model.Amount,
        Count = model.Count,
        WeightInKg = model.WeightInKg,
        ProductId = model.ProductId,
    };

    public static CostViewModel ToViewModel(this Cost cost) => new()
    {
        Id = cost.Id,
        Amount = cost.Amount,
        Count = cost.Count,
        WeightInKg = cost.WeightInKg,
        ProductId = cost.ProductId,
    };
}