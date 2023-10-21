namespace MyCosts.Api.Models.Auth;

public class AuthResponse
{
    public string? Email { get; init; }
    public string JwtToken { get; init; }

    public AuthResponse(string jwtToken)
    {
        JwtToken = jwtToken;
    }
}