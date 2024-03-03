using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyCosts.Api.Extensions;
using MyCosts.Api.Models.Config;
using MyCosts.Api.Swagger;
using MyCosts.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetRequiredSection("JwtOptions").Get<JwtOptions>()!;

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options => { options.SuppressMapClientErrors = true; });

builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(SwaggerGenBuilder.SetupSwaggerGen);

builder.Services
    .AddSingleton(jwtOptions)
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });

builder.Services
    .AddMyCostsServices(builder.Configuration)
    .AddMyCostsApiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().SetPreflightMaxAge(TimeSpan.FromDays(1)));
app.UseAuthorization();
app.MapControllers();

app.Run();