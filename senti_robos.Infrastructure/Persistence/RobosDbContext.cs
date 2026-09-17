using Microsoft.EntityFrameworkCore;
using senti_robos.Domain;

namespace senti_robos.Infrastructure.Persistence;

public class RobosDbContext(DbContextOptions<RobosDbContext> options) : DbContext(options)
{
    public DbSet<Robo> Robos => Set<Robo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RobosDbContext).Assembly);
    }
}
