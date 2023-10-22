using MyCosts.Api.Models.Receipt;
using MyCosts.Domain.Models;

namespace MyCosts.Api.Mapping;

public static class ReceiptMapping
{
    public static Receipt ToReceipt(this ReceiptEditModel model, User owner, int id = default) => new()
    {
        Id = id,
        Date = model.Date,
        PlaceName = model.PlaceName,
        UserId = owner.Id,
        Costs = model.Costs.Select(c => c.ToCost()).ToArray(),
    };

    public static ReceiptViewModel ToViewModel(this Receipt receipt) => new()
    {
        Id = receipt.Id,
        Date = receipt.Date,
        PlaceName = receipt.PlaceName,
        Costs = receipt.Costs.Select(c => c.ToViewModel()).ToArray(),
    };
}