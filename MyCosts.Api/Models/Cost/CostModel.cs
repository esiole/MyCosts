namespace MyCosts.Api.Models.Cost;

/// <summary>
///     Model for viewing cost
/// </summary>
public class CostModel
{
    /// <summary>
    ///     Amount per unit of product in rubles
    /// </summary>
    /// <example>1500.85</example>
    public decimal Amount { get; init; }

    /// <summary>
    ///     Quantity of products. The passed value will be ignored and will set to 1 if the weight parameter was passed
    /// </summary>
    /// <example>2</example>
    public int Count { get; set; }

    /// <summary>
    ///     Weight of products in kilograms. Parameter must be used if the product is sold by weight
    /// </summary>
    /// <example>4.34</example>
    public double? Weight { get; set; }

    /// <summary>
    ///     Identifier of the product to which the cost belongs
    /// </summary>
    /// <example>64</example>
    public required int ProductId { get; set; }
}