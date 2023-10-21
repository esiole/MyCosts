using MyCosts.Api.ActionFilters;
using MyCosts.Api.Services;

namespace MyCosts.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyCostsApiServices(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddTransient<IAuthService, AuthService>()
            .AddScoped<UserFilter>();
}