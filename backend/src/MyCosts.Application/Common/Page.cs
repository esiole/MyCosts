namespace MyCosts.Application.Common;

public sealed record Page<TItem, TCursor>(IReadOnlyList<TItem> Items, TCursor? NextCursor);
