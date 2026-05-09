using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.Authorization;
using MyCosts.Application.Features.Products.CreateProduct;
using MyCosts.Application.Features.Products.DeleteProduct;
using MyCosts.Application.Features.Products.GetProducts;
using MyCosts.Application.Features.Products.UpdateProduct;

namespace MyCosts.Api.Endpoints;

public static class ProductEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapProductEndpoints()
        {
            app.MapPost("/products", CreateAsync);
            app.MapPut("/products/{id:guid}", UpdateAsync);
            app.MapDelete("/products/{id:guid}", DeleteAsync);
            app.MapGet("/products", GetAsync);

            return app;
        }
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateProductRequest req,
        [FromServices] CreateProductHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        var result = await handler.HandleAsync(new CreateProductCommand(userId, req.CategoryId, req.Name), ct);

        return result.Match(
            id => Results.Created((string?)null, new { id }),
            HttpResultMapper.ToHttpResult);
    }

    private static async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest req,
        [FromServices] UpdateProductHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        var result = await handler.HandleAsync(new UpdateProductCommand(userId, id, req.CategoryId, req.Name), ct);

        return result.Match(Results.NoContent, HttpResultMapper.ToHttpResult);
    }

    private static async Task<IResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] DeleteProductHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        var result = await handler.HandleAsync(new DeleteProductCommand(userId, id), ct);

        return result.Match(Results.NoContent, HttpResultMapper.ToHttpResult);
    }

    private static async Task<IResult> GetAsync(
        [FromServices] GetProductsHandler handler,
        HttpContext httpContext,
        CancellationToken ct,
        [FromQuery] string? search = null,
        [FromQuery] string? cursor = null,
        [FromQuery, System.ComponentModel.DataAnnotations.Range(1, 100)]
        int limit = 20)
    {
        var userId = httpContext.User.GetUserId();
        var page = await handler.HandleAsync(new GetProductsQuery(userId, search, cursor, limit), ct);

        return Results.Ok(new Page<ProductResponse>(
            page.Items
                .Select(i => new ProductResponse(
                    i.Id,
                    i.Name,
                    new CategoryResponse(i.CategoryId, i.CategoryName)))
                .ToArray(),
            page.NextCursor));
    }
}

internal sealed record CreateProductRequest(Guid CategoryId, string Name);

internal sealed record UpdateProductRequest(Guid CategoryId, string Name);

internal sealed record CategoryResponse(Guid Id, string Name);

internal sealed record ProductResponse(Guid Id, string Name, CategoryResponse Category);
