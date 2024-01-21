using MyCosts.Api.Models.Cost;

namespace MyCosts.Api.Models.Receipt;

/// <summary>
///     Model for creating and editing receipt
/// </summary>
public class ReceiptEditModel
{
    /// <summary>
    ///     Date of receipt
    /// </summary>
    /// <example>2024-01-21</example>
    public DateOnly Date { get; set; }

    /// <summary>
    ///     Name of the store that issued the receipt
    /// </summary>
    /// <example>Ozon</example>
    public required string PlaceName { get; set; }

    /// <summary>
    ///     Costs included in the receipt
    /// </summary>
    public required CostModel[] Costs { get; set; }
}