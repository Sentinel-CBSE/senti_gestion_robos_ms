using senti_robos.Domain;

namespace senti_robos.Application.Interfaces;

public interface IRoboRepository
{
    Task AddAsync(Robo robo, CancellationToken cancellationToken);

    // Bounding box query backing GET api/robbery_points. Bounds are inclusive
    // and expressed as plain lat/lon ranges (see ADR note in
    // senti_robos.Infrastructure/Persistence/Configurations/RoboConfiguration.cs
    // for why this doesn't use a spatial column type).
    Task<List<Robo>> QueryAsync(
        decimal southLat,
        decimal northLat,
        decimal westLon,
        decimal eastLon,
        DateTimeOffset from,
        DateTimeOffset to,
        TipoIncidente? tipoIncidente,
        CancellationToken cancellationToken);
}
