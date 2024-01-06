using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyCostsPostgresContext(this IServiceCollection serviceCollection, string connection) =>
        serviceCollection
            .AddDbContext<PostgresContext>(optionsBuilder => optionsBuilder.UseNpgsql(connection))
            .AddSingleton<IEntityMapper<ProductCategoryEntity, ProductCategory>, ProductCategoryMapper>()
            .AddSingleton<IEntityMapper<ProductEntity, Product>, ProductMapper>()
            .AddSingleton<IEntityMapper<UserEntity, User>, UserMapper>();
}