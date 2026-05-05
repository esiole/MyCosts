using System.ComponentModel.DataAnnotations;

namespace MyCosts.Application.Features.Auth.LoginUser;

public sealed record LoginUserCommand(
    [property: Required, EmailAddress]
    string Email,
    [property: Required]
    string Password);
