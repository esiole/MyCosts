using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.ActionFilters;
using MyCosts.Api.Extensions;
using MyCosts.Api.Mapping;
using MyCosts.Api.Models.Receipt;
using MyCosts.Application.Services;

namespace MyCosts.Api.Controllers;

/// <summary>
///     Receipts management
/// </summary>
/// <response code="401">Unauthorized</response>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
[ServiceFilter(typeof(UserFilter))]
public class ReceiptsController(IReceiptService receiptService) : ControllerBase
{
    /// <summary>
    ///     Creating a new receipt
    /// </summary>
    /// <param name="body">Receipt</param>
    /// <response code="201">Receipt created successfully</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReceiptViewModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddAsync([FromBody] ReceiptEditModel body)
    {
        var createdCategory = await receiptService.AddAsync(body.ToReceipt(HttpContext.GetUser()));
        return CreatedAtAction("Get", new { createdCategory.Id }, createdCategory.ToViewModel());
    }

    /// <summary>
    ///     Editing the receipt
    /// </summary>
    /// <param name="id">Receipt identifier</param>
    /// <param name="body">Receipt new data</param>
    /// <response code="200">Receipt edited</response>
    /// <response code="404">Receipt not found</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] ReceiptEditModel body)
    {
        var editor = HttpContext.GetUser();
        var editableCategory = await receiptService.EditAsync(body.ToReceipt(editor, id), editor);
        return editableCategory == null ? NotFound() : Ok();
    }

    /// <summary>
    ///     Deleting the receipt
    /// </summary>
    /// <param name="id">Receipt identifier</param>
    /// <response code="200">Receipt deleted</response>
    /// <response code="404">Receipt not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var removedCategory = await receiptService.DeleteAsync(id, HttpContext.GetUser());
        return removedCategory == null ? NotFound() : Ok();
    }

    /// <summary>
    ///     Getting receipts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Receipts data</response>
    [HttpGet]
    [ProducesResponseType(typeof(ReceiptViewModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var receipts = await receiptService.GetAsync(HttpContext.GetUser(), cancellationToken);
        return Ok(receipts.Select(r => r.ToViewModel()));
    }

    /// <summary>
    ///     Getting the receipt
    /// </summary>
    /// <param name="id">Receipt identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Receipt data</response>
    /// <response code="404">Receipt not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReceiptViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var receipt = await receiptService.GetAsync(id, HttpContext.GetUser(), cancellationToken);
        return receipt == null ? NotFound() : Ok(receipt.ToViewModel());
    }
}