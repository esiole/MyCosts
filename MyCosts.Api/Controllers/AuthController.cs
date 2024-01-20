using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.Models.Auth;
using MyCosts.Api.Services;

namespace MyCosts.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("SignIn")]
    [AllowAnonymous]
    public async Task<IActionResult> SignInAsync([FromBody] LoginModel body)
    {
        var jwtToken = await authService.SignInAsync(body);
        return jwtToken == null ? Unauthorized() : Ok(new AuthResponse(jwtToken));
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUpAsync([FromBody] LoginModel body)
    {
        var jwtToken = await authService.SignUpAsync(body);
        return Ok(new AuthResponse(jwtToken));
    }
}