namespace MyCosts.Domain.Users;

public sealed class User
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string PasswordHash { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public User(Guid id, string email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }
}
