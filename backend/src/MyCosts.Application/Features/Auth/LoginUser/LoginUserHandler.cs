using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Domain.Users;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Application.Features.Auth.LoginUser;

public sealed class LoginUserHandler(AppDbContext db, IPasswordHasher<User> passwordHasher)
{
    public async Task<Result<User>> HandleAsync(LoginUserCommand cmd, CancellationToken ct)
    {
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(cmd, new ValidationContext(cmd), validationResults, true))
        {
            return Result<User>.Fail(ErrorKind.Unauthorized, "Invalid credentials.");
        }

        var email = cmd.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
        {
            return Result<User>.Fail(ErrorKind.Unauthorized, "Invalid credentials.");
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, cmd.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Result<User>.Fail(ErrorKind.Unauthorized, "Invalid credentials.");
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.UpdatePasswordHash(passwordHasher.HashPassword(user, cmd.Password));
            await db.SaveChangesAsync(ct);
        }

        return Result<User>.Ok(user);
    }
}
