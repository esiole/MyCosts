using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCosts.Application.Services;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Extensions;
using MyCosts.Postgres.Repositories;

namespace MyCosts.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyCostsServices(this IServiceCollection serviceCollection, IConfiguration configuration) =>
        serviceCollection
            .AddMyCostsServices()
            .AddMyCostsRepositories()
            .AddMyCostsPostgresContext(configuration.GetConnectionString("Default")!);

    private static IServiceCollection AddMyCostsServices(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddTransient<IProductCategoryService, ProductCategoryService>()
            .AddTransient<IUserService, UserService>();

    private static IServiceCollection AddMyCostsRepositories(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<IProductCategoryRepository, ProductCategoryRepository>()
            .AddScoped<IUserRepository, UserRepository>();
}