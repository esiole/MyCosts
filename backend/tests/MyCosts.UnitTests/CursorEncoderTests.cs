using MyCosts.Application.Common;
using Xunit;

namespace MyCosts.UnitTests;

public class CursorEncoderTests
{
    [Theory]
    [InlineData("Молоко")]
    [InlineData("Dairy & Eggs")]
    [InlineData("a")]
    [InlineData("категория с пробелами")]
    public void Encode_ThenDecode_ReturnsOriginalValue(string value)
    {
        var encoded = CursorEncoder.Encode(value);
        var decoded = CursorEncoder.Decode(encoded);

        Assert.Equal(value, decoded);
    }

    [Fact]
    public void Encode_ProducesUrlSafeString()
    {
        var encoded = CursorEncoder.Encode("any value");

        Assert.DoesNotContain("+", encoded);
        Assert.DoesNotContain("/", encoded);
        Assert.DoesNotContain("=", encoded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Decode_WithNullOrEmpty_ReturnsNull(string? cursor)
    {
        var result = CursorEncoder.Decode(cursor);

        Assert.Null(result);
    }

    [Fact]
    public void Decode_WithInvalidBase64_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => CursorEncoder.Decode("not-valid-base64-!!!"));
    }
}
