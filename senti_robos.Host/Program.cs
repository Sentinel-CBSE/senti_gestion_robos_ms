using Azure;
using Azure.Messaging.EventGrid;
using Scalar.AspNetCore;
using senti_robos.Application;
using senti_robos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// No authentication in this service by design: senti_api_gateway is the
// system's single point of entry and owns authentication/rate limiting.
// This service only ever gets called from inside the trust boundary
// (gateway -> this service's query endpoint, Event Grid -> this service's
// webhook), so it doesn't duplicate that concern.

builder.Services.AddApplication();
builder.AddInfrastructure();

// Dev-only: publisher client for the Event Grid custom topic, used by
// Controllers/Publisher.cs to simulate the gateway emitting events before
// senti_api_gateway exists. Not part of the real system (see design
// discussion Q13).
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var endpoint = config["EventGrid:TopicEndpoint"] ?? throw new InvalidOperationException("Missing configuration: EventGrid:TopicEndpoint");
    var key = config["EventGrid:TopicKey"] ?? throw new InvalidOperationException("Missing configuration: EventGrid:TopicKey");

    return new EventGridPublisherClient(new Uri(endpoint), new AzureKeyCredential(key));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    // APIM's importer can't parse OpenAPI 3.1's "type": ["string", "null"]
    // nullable syntax (it expects a scalar type + "nullable": true), so pin
    // the generated document to 3.0 instead of the 3.1 default.
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

var app = builder.Build();

// Brings senti_robos_db to the latest migration on startup — no separate
// migration step needed for this project's scale. Runs before the app
// starts accepting requests, so the query endpoint and webhook never see a
// stale/missing schema.
await app.MigrateDatabaseAsync();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.MapOpenApi("/openapi.json");
app.MapScalarApiReference(); // UI at /scalar/v1

// Skip HTTPS redirection in Development: ngrok (and Event Grid, Postman, etc.
// hitting it) already talk HTTPS at the edge and forward to this app over
// plain HTTP on the local port. UseHttpsRedirection would 307 those requests
// to https://localhost:<port>, which is unreachable from outside the machine.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
