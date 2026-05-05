using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCosts.Infrastructure.Jwt;
using MyCosts.Infrastructure.Persistence;

namespace MyCosts.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            services.AddJwt();

            return services;
        }

        public IServiceCollection AddPersistence(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .UseSnakeCaseNamingConvention());

            return services;
        }

        private IServiceCollection AddJwt()
        {
            services.AddOptions<JwtOptions>()
                .BindConfiguration("Jwt")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<JwtTokenService>();

            return services;
        }
    }
}
