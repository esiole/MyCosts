using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.ActionFilters;
using MyCosts.Api.Extensions;
using MyCosts.Api.Mapping;
using MyCosts.Api.Models.ProductCategory;
using MyCosts.Application.Services;
using MyCosts.Domain.Dto.Filters;

namespace MyCosts.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[ServiceFilter(typeof(UserFilter))]
public class ProductCategoriesController(IProductCategoryService productCategoryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] ProductCategoryEditModel body)
    {
        var createdCategory = await productCategoryService.AddAsync(body.ToProductCategory(HttpContext.GetUser()));
        return CreatedAtAction("Get", new { createdCategory.Id }, createdCategory.ToViewModel());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] ProductCategoryEditModel body)
    {
        var editor = HttpContext.GetUser();
        var editableCategory = await productCategoryService.EditAsync(body.ToProductCategory(editor, id), editor);
        return editableCategory == null ? NotFound() : Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var removedCategory = await productCategoryService.DeleteAsync(id, HttpContext.GetUser());
        return removedCategory == null ? NotFound() : Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] ProductCategoryFilter filter, CancellationToken cancellationToken = default)
    {
        var categories = await productCategoryService.GetAsync(filter, HttpContext.GetUser(), cancellationToken);
        return Ok(categories.Select(c => c.ToViewModel()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var category = await productCategoryService.GetAsync(id, HttpContext.GetUser(), cancellationToken);
        return category == null ? NotFound() : Ok(category.ToViewModel());
    }
}