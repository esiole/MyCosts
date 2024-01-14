using MyCosts.Api.Models.Cost;
using MyCosts.Domain.Models;

namespace MyCosts.Api.Mapping;

public static class CostMapping
{
    public static Cost ToCost(this CostModel model) => new()
    {
        Id = default,
        Amount = model.Amount,
        Count = model.Count,
        Weight = model.Weight,
        ProductId = model.ProductId,
    };

    public static CostModel ToViewModel(this Cost cost) => new()
    {
        Amount = cost.Amount,
        Count = cost.Count,
        Weight = cost.Weight,
        ProductId = cost.ProductId,
    };
}