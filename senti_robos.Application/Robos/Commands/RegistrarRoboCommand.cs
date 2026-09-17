using MediatR;

namespace senti_robos.Application.Robos.Commands;

// Shape of the "data" payload inside the Sentinel.IncidenteReportado
// EventGridEvent (see Q14 of the design discussion). Deserialized directly
// from the event's JSON data via JsonSerializerOptions.Web, so property
// names here must match the event's camelCase field names:
// { type, latitude, longitude, timestamp, reportingUserId }.
// EventGridEvent already carries its own "id" and "eventTime" in its
// envelope, so those aren't duplicated here.
public record RegistrarRoboCommand(
    string Type,
    double Latitude,
    double Longitude,
    long Timestamp,
    string ReportingUserId) : IRequest;
