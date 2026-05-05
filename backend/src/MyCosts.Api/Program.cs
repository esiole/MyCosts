using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyCosts.Api.Authorization;
using MyCosts.Api.Endpoints;
using MyCosts.Api.Middleware;
using MyCosts.Application;
using MyCosts.Infrastructure;
using MyCosts.Infrastructure.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services
    .AddOptions<JwtBearerOptions>(AuthSchemes.Bearer)
    .Configure<IOptions<JwtOptions>>((bearerOptions, jwtOptions) =>
    {
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Value.Issuer,
            ValidAudience = jwtOptions.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey))
        };
    });

builder.Services
    .AddAuthentication()
    .AddCookie(AuthSchemes.Cookie,
        options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
            options.Events.OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        })
    .AddJwtBearer(AuthSchemes.Bearer);

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(AuthSchemes.Cookie, AuthSchemes.Bearer)
        .RequireAuthenticatedUser()
        .Build();

    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(AuthSchemes.Cookie, AuthSchemes.Bearer)
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy(AuthorizationPolicies.CookieOnly,
        policy => policy
            .AddAuthenticationSchemes(AuthSchemes.Cookie)
            .RequireAuthenticatedUser());

    options.AddPolicy(AuthorizationPolicies.BearerOnly,
        policy => policy
            .AddAuthenticationSchemes(AuthSchemes.Bearer)
            .RequireAuthenticatedUser());
});

builder.Services.AddValidation();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/v1")
    .MapAuthEndpoints();

await app.RunAsync();
