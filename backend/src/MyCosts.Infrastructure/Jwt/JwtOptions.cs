using System.ComponentModel.DataAnnotations;

namespace MyCosts.Infrastructure.Jwt;

public sealed class JwtOptions : IValidatableObject
{
    [Required, MinLength(32)]
    public string SecretKey { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    public TimeSpan AccessTokenExpiry { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AccessTokenExpiry <= TimeSpan.Zero)
        {
            yield return new ValidationResult(
                "AccessTokenExpiry must be positive.",
                [nameof(AccessTokenExpiry)]);
        }
    }
}
