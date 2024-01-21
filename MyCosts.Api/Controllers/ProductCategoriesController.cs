using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.ActionFilters;
using MyCosts.Api.Extensions;
using MyCosts.Api.Mapping;
using MyCosts.Api.Models.ProductCategory;
using MyCosts.Application.Services;
using MyCosts.Domain.Dto.Filters;

namespace MyCosts.Api.Controllers;

/// <summary>
///     Product category management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[ServiceFilter(typeof(UserFilter))]
public class ProductCategoriesController(IProductCategoryService productCategoryService) : ControllerBase
{
    /// <summary>
    ///     Creating a new product category
    /// </summary>
    /// <param name="body">Product category</param>
    /// <response code="201">Product category created successfully</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductCategoryViewModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddAsync([FromBody] ProductCategoryEditModel body)
    {
        var createdCategory = await productCategoryService.AddAsync(body.ToProductCategory(HttpContext.GetUser()));
        return CreatedAtAction("Get", new { createdCategory.Id }, createdCategory.ToViewModel());
    }

    /// <summary>
    ///     Editing the product category
    /// </summary>
    /// <param name="id">Product category identifier</param>
    /// <param name="body">Product category new data</param>
    /// <response code="200">Product category edited</response>
    /// <response code="404">Product category not found</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] ProductCategoryEditModel body)
    {
        var editor = HttpContext.GetUser();
        var editableCategory = await productCategoryService.EditAsync(body.ToProductCategory(editor, id), editor);
        return editableCategory == null ? NotFound() : Ok();
    }

    /// <summary>
    ///     Deleting the product category
    /// </summary>
    /// <param name="id">Product category identifier</param>
    /// <response code="200">Product category deleted</response>
    /// <response code="404">Product category not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var removedCategory = await productCategoryService.DeleteAsync(id, HttpContext.GetUser());
        return removedCategory == null ? NotFound() : Ok();
    }

    /// <summary>
    ///     Getting product categories
    /// </summary>
    /// <param name="filter">Product category selection filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Product categories data</response>
    [HttpGet]
    [ProducesResponseType(typeof(ProductCategoryViewModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] ProductCategoryFilter filter, CancellationToken cancellationToken = default)
    {
        var categories = await productCategoryService.GetAsync(filter, HttpContext.GetUser(), cancellationToken);
        return Ok(categories.Select(c => c.ToViewModel()));
    }

    /// <summary>
    ///     Getting the product category
    /// </summary>
    /// <param name="id">Product category identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Product category data</response>
    /// <response code="404">Product category not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductCategoryViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var category = await productCategoryService.GetAsync(id, HttpContext.GetUser(), cancellationToken);
        return category == null ? NotFound() : Ok(category.ToViewModel());
    }
}