using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyCosts.Api.Mapping;
using MyCosts.Api.Models.Auth;
using MyCosts.Api.Models.Config;
using MyCosts.Application.Services;
using MyCosts.Domain.Models;

namespace MyCosts.Api.Services;

public interface IAuthService
{
    Task<string?> SignInAsync(LoginModel loginModel);
    Task<string> SignUpAsync(LoginModel loginModel);
}

public class AuthService(JwtOptions jwtOptions, IUserService userService) : IAuthService
{
    public async Task<string?> SignInAsync(LoginModel loginModel)
    {
        var user = await userService.GetAsync(loginModel.Email, loginModel.Password);
        return user == null ? null : CreateJwtToken(user);
    }

    public async Task<string> SignUpAsync(LoginModel loginModel)
    {
        var user = await userService.AddAsync(loginModel.ToNewUser());
        return CreateJwtToken(user);
    }

    private string CreateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var securityToken = new JwtSecurityToken(
            jwtOptions.Issuer,
            jwtOptions.Audience,
            claims,
            expires: DateTime.Now.AddDays(jwtOptions.ValidForDays),
            signingCredentials: credentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return jwtToken;
    }
}