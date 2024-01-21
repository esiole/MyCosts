namespace MyCosts.Api.Models.Auth;

/// <summary>
///     Authorization data
/// </summary>
public class AuthResponse
{
    /// <summary>
    ///     Authed user email
    /// </summary>
    /// <example>testtesttest@gmail.com</example>
    public string Email { get; init; }

    /// <summary>
    ///     JWT token for access to API
    /// </summary>
    /// <example>jwt</example>
    public string JwtToken { get; init; }

    public AuthResponse(string email, string jwtToken)
    {
        Email = email;
        JwtToken = jwtToken;
    }
}