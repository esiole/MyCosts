using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.ActionFilters;
using MyCosts.Api.Extensions;
using MyCosts.Api.Mapping;
using MyCosts.Api.Models.Product;
using MyCosts.Application.Services;

namespace MyCosts.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[ServiceFilter(typeof(UserFilter))]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] ProductEditModel body)
    {
        var createdProduct = await _productService.AddAsync(body.ToProduct());
        return CreatedAtAction("Get", new { createdProduct.Id }, createdProduct.ToViewModel());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] ProductEditModel body)
    {
        var editor = HttpContext.GetUser();
        var editableProduct = await _productService.EditAsync(body.ToProduct(id), editor);
        return editableProduct == null ? NotFound() : Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var removedProduct = await _productService.DeleteAsync(id, HttpContext.GetUser());
        return removedProduct == null ? NotFound() : Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetAsync(HttpContext.GetUser(), cancellationToken);
        return Ok(products.Select(p => p.ToViewModel()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetAsync(id, HttpContext.GetUser(), cancellationToken);
        return product == null ? NotFound() : Ok(product.ToViewModel());
    }
}