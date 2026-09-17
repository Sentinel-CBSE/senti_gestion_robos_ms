using Microsoft.EntityFrameworkCore;
using senti_robos.Application.Interfaces;
using senti_robos.Domain;

namespace senti_robos.Infrastructure.Persistence;

public class RoboRepository(RobosDbContext dbContext) : IRoboRepository
{
    public async Task AddAsync(Robo robo, CancellationToken cancellationToken)
    {
        dbContext.Robos.Add(robo);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Robo>> QueryAsync(
        decimal southLat,
        decimal northLat,
        decimal westLon,
        decimal eastLon,
        DateTimeOffset from,
        DateTimeOffset to,
        TipoIncidente? tipoIncidente,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Robos
            .AsNoTracking()
            .Where(r => r.Latitud >= southLat && r.Latitud <= northLat)
            .Where(r => r.Longitud >= westLon && r.Longitud <= eastLon)
            .Where(r => r.FechaHoraIncidente >= from && r.FechaHoraIncidente <= to);

        if (tipoIncidente is not null)
            query = query.Where(r => r.TipoIncidente == tipoIncidente);

        return await query.ToListAsync(cancellationToken);
    }
}
