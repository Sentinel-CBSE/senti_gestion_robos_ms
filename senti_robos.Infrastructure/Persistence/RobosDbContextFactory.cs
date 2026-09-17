using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace senti_robos.Infrastructure.Persistence;

// Design-time only: lets `dotnet ef migrations add` build a RobosDbContext
// without spinning up the whole Aspire-orchestrated host (which is how the
// connection string is actually resolved at runtime — see
// DependencyInjection.AddInfrastructure). The connection string here is
// never used to run the app, only to generate migration files.
public class RobosDbContextFactory : IDesignTimeDbContextFactory<RobosDbContext>
{
    public RobosDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RobosDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=sentirobosdb;Trusted_Connection=True;TrustServerCertificate=True");

        return new RobosDbContext(optionsBuilder.Options);
    }
}
