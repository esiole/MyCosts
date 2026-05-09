using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.Products.GetProducts;

public sealed class GetProductsHandler(AppDbContext db)
{
    public async Task<Page<ProductSummary, string>> HandleAsync(GetProductsQuery query, CancellationToken ct)
    {
        var cursor = CursorEncoder.Decode(query.Cursor);
        var search = query.Search?.Trim().ToLowerInvariant();

        var join = db.Products
            .Join(db.ProductCategories, p => p.CategoryId, c => c.Id, (p, c) => new { p, c })
            .Where(t => t.c.UserId == query.UserId)
            .Select(t => new { Product = t.p, Category = t.c });

        if (!string.IsNullOrEmpty(search))
        {
            join = join.Where(x => x.Product.Name.ToLower().StartsWith(search));
        }

        if (cursor is not null)
        {
            join = join.Where(x => x.Product.Name.CompareTo(cursor) > 0);
        }

        var baseQuery = join
            .OrderBy(x => x.Product.Name)
            .Select(x => new ProductSummary(x.Product.Id, x.Product.Name, x.Category.Id, x.Category.Name));

        if (query.Limit is null)
        {
            var allItems = await baseQuery.ToListAsync(ct);
            return new Page<ProductSummary, string>(allItems, null);
        }

        var limit = query.Limit.Value;
        var items = await baseQuery.Take(limit + 1).ToListAsync(ct);

        string? nextCursor = null;
        if (items.Count > limit)
        {
            items.RemoveAt(items.Count - 1);
            nextCursor = CursorEncoder.Encode(items[^1].Name);
        }

        return new Page<ProductSummary, string>(items, nextCursor);
    }
}
