using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MyCosts.Domain.Users;

namespace MyCosts.Infrastructure.Jwt;

public sealed class JwtTokenService(IOptions<JwtOptions> options)
{
    private static readonly JsonWebTokenHandler _tokenHandler = new();

    public string Generate(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                [JwtRegisteredClaimNames.Email] = user.Email,
            },
            Expires = DateTimeOffset.UtcNow.Add(options.Value.AccessTokenExpiry).UtcDateTime,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        };

        return _tokenHandler.CreateToken(descriptor);
    }
}
