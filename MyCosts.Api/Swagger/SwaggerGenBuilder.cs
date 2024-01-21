using System.Reflection;
using Microsoft.OpenApi.Models;
using MyCosts.Api.Swagger.OperationFilters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyCosts.Api.Swagger;

public static class SwaggerGenBuilder
{
    public static void SetupSwaggerGen(SwaggerGenOptions options)
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
    }
}