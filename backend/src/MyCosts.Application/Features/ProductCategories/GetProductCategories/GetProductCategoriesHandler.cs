using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Domain.Products;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.ProductCategories.GetProductCategories;

public sealed class GetProductCategoriesHandler(AppDbContext db)
{
    public async Task<Page<ProductCategory, string>> HandleAsync(GetProductCategoriesQuery query, CancellationToken ct)
    {
        var cursor = CursorEncoder.Decode(query.Cursor);
        var search = query.Search?.Trim().ToLowerInvariant();

        var q = db.ProductCategories.Where(c => c.UserId == query.UserId);

        if (!string.IsNullOrEmpty(search))
        {
            q = q.Where(c => c.Name.ToLower().StartsWith(search));
        }

        if (cursor is not null)
        {
            q = q.Where(c => c.Name.CompareTo(cursor) > 0);
        }

        var baseQuery = q.OrderBy(c => c.Name);

        if (query.Limit is null)
        {
            var allItems = await baseQuery.ToListAsync(ct);
            return new Page<ProductCategory, string>(allItems, null);
        }

        var limit = query.Limit.Value;
        var items = await baseQuery.Take(limit + 1).ToListAsync(ct);

        string? nextCursor = null;
        if (items.Count > limit)
        {
            items.RemoveAt(items.Count - 1);
            nextCursor = CursorEncoder.Encode(items[^1].Name);
        }

        return new Page<ProductCategory, string>(items, nextCursor);
    }
}
