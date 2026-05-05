using MyCosts.Domain.Users;

namespace MyCosts.UnitTests.Builders;

public sealed class UserBuilder
{
    private Guid _id = Guid.CreateVersion7();
    private string _email = "user@example.com";
    private string _passwordHash = "hashed-password";
    private DateTimeOffset _createdAt = new(2026, 5, 5, 22, 39, 0, TimeSpan.Zero);

    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public UserBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public User Build() => new(_id, _email, _passwordHash, _createdAt);
}
