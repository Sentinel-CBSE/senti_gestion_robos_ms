using MediatR;
using senti_robos.Application.Common;
using senti_robos.Application.Interfaces;
using senti_robos.Domain;

namespace senti_robos.Application.Robos.Commands;

public class RegistrarRoboCommandHandler(IRoboRepository roboRepository)
    : IRequestHandler<RegistrarRoboCommand>
{
    public async Task Handle(RegistrarRoboCommand request, CancellationToken cancellationToken)
    {
        if (!TipoIncidenteExtensions.TryParseWireValue(request.Type, out var tipoIncidente))
            throw new InvalidOperationException($"Tipo de incidente desconocido en el evento: '{request.Type}'.");

        var fechaHoraIncidente = DateTimeOffset.FromUnixTimeMilliseconds(request.Timestamp);

        var robo = Robo.Crear(
            tipoIncidente,
            (decimal)request.Latitude,
            (decimal)request.Longitude,
            fechaHoraIncidente,
            request.ReportingUserId);

        await roboRepository.AddAsync(robo, cancellationToken);
    }
}
