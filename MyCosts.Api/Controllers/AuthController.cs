using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.Models.Auth;
using MyCosts.Api.Services;

namespace MyCosts.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("SignIn")]
    [AllowAnonymous]
    public async Task<IActionResult> SignInAsync([FromBody] LoginModel body)
    {
        var jwtToken = await _authService.SignInAsync(body);
        return jwtToken == null ? Unauthorized() : Ok(new AuthResponse(jwtToken));
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUpAsync([FromBody] LoginModel body)
    {
        var jwtToken = await _authService.SignUpAsync(body);
        return Ok(new AuthResponse(jwtToken));
    }
}