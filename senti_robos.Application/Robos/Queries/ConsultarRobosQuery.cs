using MediatR;
using senti_robos.Application.Robos.Dtos;
using senti_robos.Domain;

namespace senti_robos.Application.Robos.Queries;

// Backs GET api/robbery_points. Field names mirror the query params
// senti_mobile already sends (northLat, southLat, eastLon, westLon, from,
// to, type) — see SentinelApi.kt. `TipoIncidente` here is already parsed
// and validated by the controller before dispatch.
public record ConsultarRobosQuery(
    double NorthLat,
    double SouthLat,
    double EastLon,
    double WestLon,
    long From,
    long To,
    TipoIncidente? TipoIncidente) : IRequest<List<RobberyPointDto>>;
