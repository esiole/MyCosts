using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.ActionFilters;
using MyCosts.Api.Extensions;
using MyCosts.Api.Mapping;
using MyCosts.Api.Models.Receipt;
using MyCosts.Application.Services;

namespace MyCosts.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[ServiceFilter(typeof(UserFilter))]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] ReceiptEditModel body)
    {
        var createdCategory = await _receiptService.AddAsync(body.ToReceipt(HttpContext.GetUser()));
        return CreatedAtAction("Get", new { createdCategory.Id }, createdCategory.ToViewModel());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] ReceiptEditModel body)
    {
        var editor = HttpContext.GetUser();
        var editableCategory = await _receiptService.EditAsync(body.ToReceipt(editor, id), editor);
        return editableCategory == null ? NotFound() : Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var removedCategory = await _receiptService.DeleteAsync(id, HttpContext.GetUser());
        return removedCategory == null ? NotFound() : Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var receipts = await _receiptService.GetAsync(HttpContext.GetUser(), cancellationToken);
        return Ok(receipts.Select(r => r.ToViewModel()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var receipt = await _receiptService.GetAsync(id, HttpContext.GetUser(), cancellationToken);
        return receipt == null ? NotFound() : Ok(receipt.ToViewModel());
    }
}