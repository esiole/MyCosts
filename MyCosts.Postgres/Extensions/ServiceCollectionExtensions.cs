using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MyCosts.Postgres.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyCostsPostgresContext(this IServiceCollection serviceCollection, string connection) =>
        serviceCollection.AddDbContext<PostgresContext>(optionsBuilder => optionsBuilder.UseNpgsql(connection));
}