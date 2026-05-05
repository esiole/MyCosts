using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCosts.Infrastructure;
using MyCosts.Infrastructure.Persistence;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) => services.AddPersistence(ctx.Configuration))
    .Build();

await using var scope = host.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

Console.WriteLine("Applying migrations...");
await db.Database.MigrateAsync();
Console.WriteLine("Migrations applied.");
