using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.Authorization;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.ProductCategories.DeleteProductCategory;
using MyCosts.Application.Features.ProductCategories.GetProductCategories;
using MyCosts.Application.Features.ProductCategories.UpdateProductCategory;

namespace MyCosts.Api.Endpoints;

public static class ProductCategoryEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapProductCategoryEndpoints()
        {
            app.MapPost("/categories", CreateAsync);
            app.MapPut("/categories/{id:guid}", UpdateAsync);
            app.MapDelete("/categories/{id:guid}", DeleteAsync);
            app.MapGet("/categories", GetAsync);

            return app;
        }
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateCategoryRequest req,
        [FromServices] CreateProductCategoryHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        var result = await handler.HandleAsync(new CreateProductCategoryCommand(userId, req.Name), ct);

        return result.Match(
            id => Results.Created((string?)null, new { id }),
            HttpResultMapper.ToHttpResult);
    }

    private static async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest req,
        [FromServices] UpdateProductCategoryHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        var result = await handler.HandleAsync(new UpdateProductCategoryCommand(userId, id, req.Name), ct);

        return result.Match(Results.NoContent, HttpResultMapper.ToHttpResult);
    }

    private static async Task<IResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] DeleteProductCategoryHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        var result = await handler.HandleAsync(new DeleteProductCategoryCommand(userId, id), ct);

        return result.Match(Results.NoContent, HttpResultMapper.ToHttpResult);
    }

    private static async Task<IResult> GetAsync(
        [FromServices] GetProductCategoriesHandler handler,
        HttpContext httpContext,
        CancellationToken ct,
        [FromQuery] string? search = null,
        [FromQuery] string? cursor = null,
        [FromQuery, System.ComponentModel.DataAnnotations.Range(1, 100)]
        int limit = 20)
    {
        var userId = httpContext.User.GetUserId();
        var page = await handler.HandleAsync(new GetProductCategoriesQuery(userId, search, cursor, limit), ct);

        return Results.Ok(new Page<ProductCategoryResponse>(
            page.Items.Select(c => new ProductCategoryResponse(c.Id, c.Name)).ToList(),
            page.NextCursor));
    }
}

internal sealed record CreateCategoryRequest(string Name);

internal sealed record UpdateCategoryRequest(string Name);

internal sealed record ProductCategoryResponse(Guid Id, string Name);
