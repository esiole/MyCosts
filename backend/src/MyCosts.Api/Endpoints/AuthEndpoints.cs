using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyCosts.Api.Authorization;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.LoginUser;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Domain.Users;
using MyCosts.Infrastructure.Jwt;

namespace MyCosts.Api.Endpoints;

public static class AuthEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapAuthEndpoints()
        {
            app.MapPost("/auth/register", RegisterAsync)
                .AllowAnonymous();

            app.MapPost("/auth/login", LoginAsync)
                .AllowAnonymous();

            app.MapPost("/auth/logout", (Delegate)LogoutAsync)
                .RequireAuthorization(AuthorizationPolicies.CookieOnly);

            app.MapPost("/auth/mobile/login", MobileLoginAsync)
                .AllowAnonymous();

            app.MapPost("/auth/mobile/logout", MobileLogout)
                .RequireAuthorization(AuthorizationPolicies.BearerOnly);

            return app;
        }
    }

    private static async Task<IResult> RegisterAsync(
        [FromBody] RegisterRequest req,
        [FromServices] RegisterUserHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new RegisterUserCommand(req.Email, req.Password), ct);

        return result.Match(_ => Results.Ok(), ToHttpResult);
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest req,
        [FromServices] LoginUserHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new LoginUserCommand(req.Email, req.Password), ct);

        if (!result.IsSuccess)
        {
            return ToHttpResult(result.Error);
        }

        var user = result.Value;
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };
        var identity = new ClaimsIdentity(claims, AuthSchemes.Cookie);
        var principal = new ClaimsPrincipal(identity);
        await httpContext.SignInAsync(AuthSchemes.Cookie, principal);

        return Results.Ok();
    }

    private static async Task<IResult> LogoutAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(AuthSchemes.Cookie);

        return Results.Ok();
    }

    private static async Task<IResult> MobileLoginAsync(
        [FromBody] LoginRequest req,
        [FromServices] LoginUserHandler handler,
        [FromServices] JwtTokenService tokenService,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new LoginUserCommand(req.Email, req.Password), ct);

        return result.Match(
            user => Results.Ok(new { token = tokenService.Generate(user) }),
            ToHttpResult);
    }

    private static IResult MobileLogout() => Results.Ok();

    private static IResult ToHttpResult(AppError error) => error.Kind switch
    {
        ErrorKind.Validation => Results.ValidationProblem(
            error.ValidationErrors?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            ?? new Dictionary<string, string[]> { [""] = [error.Message] }),
        ErrorKind.NotFound => Results.Problem(title: error.Message, statusCode: StatusCodes.Status404NotFound),
        ErrorKind.Conflict => Results.Problem(title: error.Message, statusCode: StatusCodes.Status409Conflict),
        ErrorKind.Unauthorized => Results.Problem(
            title: "Invalid credentials.",
            statusCode: StatusCodes.Status401Unauthorized),
        _ => Results.Problem(),
    };
}

internal sealed record RegisterRequest(
    [Required, EmailAddress, MaxLength(UserConstraints.EmailMaxLength)]
    string Email,
    [Required, MinLength(UserConstraints.PasswordMinLength), MaxLength(UserConstraints.PasswordMaxLength)]
    string Password);

internal sealed record LoginRequest(
    [Required, EmailAddress]
    string Email,
    [Required]
    string Password);
