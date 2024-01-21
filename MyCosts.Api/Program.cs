using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyCosts.Api.Extensions;
using MyCosts.Api.Models.Config;
using MyCosts.Api.Swagger.OperationFilters;
using MyCosts.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetRequiredSection("JwtOptions").Get<JwtOptions>()!;

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options => { options.SuppressMapClientErrors = true; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyCosts API",
        Version = "v1",
        Description = "MyCosts API documentation",
        Contact = new OpenApiContact
        {
            Name = "Oleg Esikov",
            Email = "esikov.oleg@mail.ru",
            Url = new Uri("https://github.com/esiole"),
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT"),
        },
    });

    var xmlDocFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlDocFileName));

    options.SupportNonNullableReferenceTypes();

    options.AddSecurityDefinition(SecureEndpointAuthRequirementFilter.SchemeName, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below",
    });

    options.OperationFilter<SecureEndpointAuthRequirementFilter>();
});

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
app.UseAuthorization();
app.MapControllers();

app.Run();