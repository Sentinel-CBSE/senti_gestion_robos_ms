using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using senti_robos.Application.Robos.Commands;

namespace senti_robos.Host.Controllers;

// Real Event Grid webhook for senti_eventos_mq (FR-01 F2). Replaces the old
// throwaway Test.cs — same validation-handshake handling that was already
// proven end to end via ngrok (see design discussion Q13), but wired to the
// actual domain event instead of a placeholder. Fed either by the future
// senti_api_gateway or, locally, by Controllers/Publisher.cs.
[ApiController]
[Route("webhooks/incidentes")]
public class IncidentesWebhookController(IMediator mediator, ILogger<IncidentesWebhookController> logger)
    : ControllerBase
{
    private const string ValidationEventType = "Microsoft.EventGrid.SubscriptionValidationEvent";
    private const string IncidenteReportadoEventType = "Sentinel.IncidenteReportado";

    private static readonly ActivitySource ActivitySource = new("senti_robos.Host");

    // Event Grid's webhook handler always uses HTTP POST — both for the
    // subscription validation handshake and for real event delivery.
    [HttpPost]
    public async Task<IActionResult> Receive(CancellationToken cancellationToken)
    {
        var body = await BinaryData.FromStreamAsync(Request.Body, cancellationToken);
        var events = EventGridEvent.ParseMany(body);

        foreach (var evt in events)
        {
            using var activity = ActivitySource.StartActivity($"EventGrid.{evt.EventType}");
            activity?.SetTag("eventgrid.event_type", evt.EventType);
            activity?.SetTag("eventgrid.id", evt.Id);
            activity?.SetTag("eventgrid.subject", evt.Subject);
            activity?.SetTag("eventgrid.topic", evt.Topic);

            if (evt.EventType == ValidationEventType)
            {
                var validationData = evt.Data!.ToObjectFromJson<SubscriptionValidationEventData>()
                    ?? throw new InvalidOperationException("Subscription validation event had no data.");
                logger.LogInformation("Event Grid subscription validation handshake received.");

                // This exact shape, with a 200, is what Event Grid waits for
                // to complete the subscription.
                return Ok(new SubscriptionValidationResponse
                {
                    ValidationResponse = validationData.ValidationCode
                });
            }

            if (evt.EventType != IncidenteReportadoEventType)
            {
                logger.LogWarning(
                    "Ignoring unrecognized event. EventType={EventType}, Id={Id}",
                    evt.EventType, evt.Id);
                continue;
            }

            var command = evt.Data!.ToObjectFromJson<RegistrarRoboCommand>(JsonSerializerOptions.Web)
                ?? throw new InvalidOperationException($"Evento {evt.Id} de tipo {IncidenteReportadoEventType} no traía datos.");

            logger.LogInformation(
                "Registrando robo desde evento {Id}. Type={Type}", evt.Id, command.Type);

            await mediator.Send(command, cancellationToken);
        }

        return Ok();
    }
}
