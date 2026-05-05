using MyCosts.UnitTests.Builders;
using Xunit;

namespace MyCosts.UnitTests.Domain.Users;

public sealed class UserTests
{
    [Fact]
    public void UpdatePasswordHash_SetsNewHash()
    {
        var user = new UserBuilder().WithPasswordHash("old-hash").Build();

        user.UpdatePasswordHash("new-hash");

        Assert.Equal("new-hash", user.PasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdatePasswordHash_WithInvalidHash_ThrowsArgumentException(string invalidHash)
    {
        var user = new UserBuilder().Build();

        Assert.Throws<ArgumentException>(() => user.UpdatePasswordHash(invalidHash));
    }
}
