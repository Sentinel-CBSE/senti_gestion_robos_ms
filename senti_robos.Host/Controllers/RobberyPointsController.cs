using MediatR;
using Microsoft.AspNetCore.Mvc;
using senti_robos.Application.Common;
using senti_robos.Application.Robos.Dtos;
using senti_robos.Application.Robos.Queries;
using senti_robos.Domain;

namespace senti_robos.Host.Controllers;

// FR-06 F2: "consultar en memoria de robos", called by the gateway on a
// metrics-cache miss. Route/query-param names/response shape mirror
// senti_mobile's SentinelApi.getRobberyPoints exactly (design discussion
// Q2), so the gateway can reverse-proxy without translating anything.
[ApiController]
public class RobberyPointsController(IMediator mediator) : ControllerBase
{
    [HttpGet("/list")]
    public async Task<ActionResult<List<RobberyPointDto>>> GetRobberyPoints(
        [FromQuery] double northLat,
        [FromQuery] double southLat,
        [FromQuery] double eastLon,
        [FromQuery] double westLon,
        [FromQuery] long from,
        [FromQuery] long to,
        [FromQuery] string? type,
        CancellationToken cancellationToken)
    {
        TipoIncidente? tipoIncidente = null;
        if (type is not null)
        {
            if (!TipoIncidenteExtensions.TryParseWireValue(type, out var parsed))
                return BadRequest($"Tipo de incidente inválido: '{type}'.");

            tipoIncidente = parsed;
        }

        var query = new ConsultarRobosQuery(northLat, southLat, eastLon, westLon, from, to, tipoIncidente);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}
