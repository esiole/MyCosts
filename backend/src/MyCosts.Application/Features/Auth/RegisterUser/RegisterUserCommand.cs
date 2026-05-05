using System.ComponentModel.DataAnnotations;
using MyCosts.Domain.Users;

namespace MyCosts.Application.Features.Auth.RegisterUser;

public sealed record RegisterUserCommand(
    [property: Required, EmailAddress]
    string Email,
    [property: Required, MinLength(UserConstraints.PasswordMinLength)]
    string Password);
