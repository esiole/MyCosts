using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.ActionFilters;
using MyCosts.Api.Extensions;
using MyCosts.Api.Mapping;
using MyCosts.Api.Models.Product;
using MyCosts.Application.Services;

namespace MyCosts.Api.Controllers;

/// <summary>
///     Products management
/// </summary>
/// <response code="401">Unauthorized</response>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
[ServiceFilter(typeof(UserFilter))]
public class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>
    ///     Creating a new product
    /// </summary>
    /// <param name="body">Product</param>
    /// <response code="201">Product created successfully</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddAsync([FromBody] ProductEditModel body)
    {
        var createdProduct = await productService.AddAsync(body.ToProduct());
        return CreatedAtAction("Get", new { createdProduct.Id }, createdProduct.ToViewModel());
    }

    /// <summary>
    ///     Editing the product
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <param name="body">Product new data</param>
    /// <response code="200">Product edited</response>
    /// <response code="404">Product not found</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] ProductEditModel body)
    {
        var editor = HttpContext.GetUser();
        var editableProduct = await productService.EditAsync(body.ToProduct(id), editor);
        return editableProduct == null ? NotFound() : Ok();
    }

    /// <summary>
    ///     Deleting the product
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <response code="200">Product deleted</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var removedProduct = await productService.DeleteAsync(id, HttpContext.GetUser());
        return removedProduct == null ? NotFound() : Ok();
    }

    /// <summary>
    ///     Getting products
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Products data</response>
    [HttpGet]
    [ProducesResponseType(typeof(ProductViewModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAsync(HttpContext.GetUser(), cancellationToken);
        return Ok(products.Select(p => p.ToViewModel()));
    }

    /// <summary>
    ///     Getting the product
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Product data</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var product = await productService.GetAsync(id, HttpContext.GetUser(), cancellationToken);
        return product == null ? NotFound() : Ok(product.ToViewModel());
    }
}