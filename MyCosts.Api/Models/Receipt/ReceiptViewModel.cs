using MyCosts.Api.Models.Cost;

namespace MyCosts.Api.Models.Receipt;

/// <summary>
///     Model for viewing receipt
/// </summary>
public class ReceiptViewModel
{
    /// <summary>
    ///     Receipt identifier
    /// </summary>
    /// <example>239</example>
    public int Id { get; set; }

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