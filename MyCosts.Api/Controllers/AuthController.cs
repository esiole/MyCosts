using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.Models.Auth;
using MyCosts.Api.Services;

namespace MyCosts.Api.Controllers;

/// <summary>
///     Authorization management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    ///     Login to the application
    /// </summary>
    /// <param name="body">Login data</param>
    /// <response code="200">Auth data</response>
    /// <response code="401">Incorrect login details</response>
    [HttpPost("SignIn")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignInAsync([FromBody] LoginModel body)
    {
        var jwtToken = await authService.SignInAsync(body);
        return jwtToken == null ? Unauthorized() : Ok(new AuthResponse(body.Email, jwtToken));
    }

    /// <summary>
    ///     New user registration
    /// </summary>
    /// <param name="body">Login data</param>
    /// <response code="200">Auth data</response>
    [HttpPost("SignUp")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignUpAsync([FromBody] LoginModel body)
    {
        var jwtToken = await authService.SignUpAsync(body);
        return Ok(new AuthResponse(body.Email, jwtToken));
    }
}