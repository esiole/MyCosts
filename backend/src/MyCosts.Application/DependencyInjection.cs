using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyCosts.Application.Features.Auth.LoginUser;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Domain.Users;

namespace MyCosts.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddScoped<RegisterUserHandler>();
            services.AddScoped<LoginUserHandler>();

            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

            return services;
        }
    }
}
