using MyCosts.Api.Models.Product;

namespace MyCosts.Api.Models.ProductCategory;

/// <summary>
///     Model for viewing product category
/// </summary>
public class ProductCategoryViewModel
{
    /// <summary>
    ///     Product category identifier
    /// </summary>
    /// <example>168</example>
    public int Id { get; init; }

    /// <summary>
    ///     Product category name
    /// </summary>
    /// <example>Medicines</example>
    public required string Name { get; init; }

    /// <summary>
    ///     Products that belong to category
    /// </summary>
    public required ProductViewModel[] Products { get; init; }
}