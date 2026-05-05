using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Domain.Users;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.Auth.RegisterUser;

public sealed class RegisterUserHandler(AppDbContext db, IPasswordHasher<User> passwordHasher)
{
    // PasswordHasher<T> ignores the user argument; a sentinel avoids null! suppression.
    private static readonly User _hashingContext = new(Guid.Empty, string.Empty, string.Empty, default);

    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand cmd, CancellationToken ct)
    {
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(cmd, new ValidationContext(cmd), validationResults, true))
        {
            return Result<Guid>.ValidationFail(validationResults);
        }

        var email = cmd.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Email == email, ct))
        {
            return Result<Guid>.Fail(ErrorKind.Conflict, "Email is already taken.");
        }

        var user = new User(
            Guid.CreateVersion7(),
            email,
            passwordHasher.HashPassword(_hashingContext, cmd.Password),
            DateTimeOffset.UtcNow
        );

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(user.Id);
    }
}
