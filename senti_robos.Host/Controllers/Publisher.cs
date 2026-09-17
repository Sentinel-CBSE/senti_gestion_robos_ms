using System.Diagnostics;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc;

namespace senti_robos.Host.Controllers
{
    // Dev-only tool: stands in for the gateway, which is what will actually
    // publish IncidenteReportado events once it exists. Lets you exercise the
    // full round trip (this service -> Event Grid -> IncidentesWebhookController)
    // locally without depending on senti_api_gateway being built yet. Not part
    // of the real system — see design discussion Q13.
    [ApiController]
    [Route("[controller]")]
    public class Publisher : ControllerBase
    {
        private static readonly ActivitySource ActivitySource = new("senti_robos.Host");

        private readonly EventGridPublisherClient _client;
        private readonly ILogger<Publisher> _logger;

        public Publisher(EventGridPublisherClient client, ILogger<Publisher> logger)
        {
            _client = client;
            _logger = logger;
        }

        public record IncidenteReportadoTestRequest(
            string Type,
            double Latitude,
            double Longitude,
            long? Timestamp,
            string? ReportingUserId);

        // POST /publisher/incidente-reportado
        // Simulates what the real gateway will do once it exists: publish a
        // Sentinel.IncidenteReportado event carrying the same "data" shape
        // IncidentesWebhookController expects (RegistrarRoboCommand). Fire
        // this to exercise the full round trip end to end — this service ->
        // Event Grid -> IncidentesWebhookController -> senti_robos_db —
        // without needing senti_api_gateway to be built yet.
        [HttpPost("incidente-reportado")]
        public async Task<IActionResult> PublishIncidenteReportado([FromBody] IncidenteReportadoTestRequest request)
        {
            var evt = new EventGridEvent(
                subject: $"incidentes/{request.Type}",
                eventType: "Sentinel.IncidenteReportado",
                dataVersion: "1.0",
                data: new
                {
                    type = request.Type,
                    latitude = request.Latitude,
                    longitude = request.Longitude,
                    timestamp = request.Timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    reportingUserId = request.ReportingUserId ?? "test-user"
                });

            using var activity = ActivitySource.StartActivity("EventGrid.Publish");
            activity?.SetTag("eventgrid.event_type", evt.EventType);
            activity?.SetTag("eventgrid.subject", evt.Subject);
            activity?.SetTag("eventgrid.id", evt.Id);

            _logger.LogInformation(
                "Publishing IncidenteReportado test event. Id={Id}, Subject={Subject}",
                evt.Id, evt.Subject);

            await _client.SendEventAsync(evt);

            return Accepted(new { evt.Id, evt.EventType, evt.Subject });
        }
    }
}
