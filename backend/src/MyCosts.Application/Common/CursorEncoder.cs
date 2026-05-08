using System.Buffers.Text;
using System.Text;

namespace MyCosts.Application.Common;

internal static class CursorEncoder
{
    public static string Encode(string value) => Base64Url.EncodeToString(Encoding.UTF8.GetBytes(value));

    public static string? Decode(string? cursor) =>
        string.IsNullOrEmpty(cursor)
            ? null
            : Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor));
}
