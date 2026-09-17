using MediatR;
using senti_robos.Application.Common;
using senti_robos.Application.Interfaces;
using senti_robos.Application.Robos.Dtos;

namespace senti_robos.Application.Robos.Queries;

public class ConsultarRobosQueryHandler(IRoboRepository roboRepository)
    : IRequestHandler<ConsultarRobosQuery, List<RobberyPointDto>>
{
    public async Task<List<RobberyPointDto>> Handle(ConsultarRobosQuery request, CancellationToken cancellationToken)
    {
        var from = DateTimeOffset.FromUnixTimeMilliseconds(request.From);
        var to = DateTimeOffset.FromUnixTimeMilliseconds(request.To);

        var robos = await roboRepository.QueryAsync(
            (decimal)request.SouthLat,
            (decimal)request.NorthLat,
            (decimal)request.WestLon,
            (decimal)request.EastLon,
            from,
            to,
            request.TipoIncidente,
            cancellationToken);

        return robos
            .Select(robo => new RobberyPointDto(
                robo.Id.ToString(),
                (double)robo.Latitud,
                (double)robo.Longitud,
                robo.FechaHoraIncidente.ToUnixTimeMilliseconds(),
                robo.TipoIncidente.ToWireValue()))
            .ToList();
    }
}
