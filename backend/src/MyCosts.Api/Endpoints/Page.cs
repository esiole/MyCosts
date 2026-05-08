namespace MyCosts.Api.Endpoints;

public sealed record Page<T>(IReadOnlyList<T> Items, string? NextCursor);
