using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using senti_robos.Application.Interfaces;
using senti_robos.Infrastructure.Persistence;

namespace senti_robos.Infrastructure;

public static class DependencyInjection
{
    // "sentirobosdb" is the Aspire resource/connection name — the AppHost
    // (senti_robos.AppHost/AppHost.cs) provisions a local SQL Server
    // container under this name for development; in the deployed
    // environment it's whatever connection string / Managed Identity
    // config is set under ConnectionStrings:sentirobosdb.
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<RobosDbContext>("sentirobosdb");

        builder.Services.AddScoped<IRoboRepository, RoboRepository>();

        return builder;
    }

    // Applies any pending migrations on startup (creates the database and/or
    // brings its schema to the latest migration). Call this once, right
    // after `builder.Build()`, before the app starts serving requests.
    public static async Task MigrateDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RobosDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
