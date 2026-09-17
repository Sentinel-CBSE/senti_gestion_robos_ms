namespace senti_robos.Domain;

// Aggregate root for the "Gestión de Robos" bounded context. Instances are
// only ever created from the IncidenteReportado event (see
// senti_robos.Application/Robos/Commands/RegistrarRoboCommand.cs) — there is
// no direct public "create" API, matching FR-01: the mobile app reports
// through the gateway, which publishes the event; this service only reacts.
public class Robo
{
    public Guid Id { get; private set; }
    public TipoIncidente TipoIncidente { get; private set; }
    public string? Descripcion { get; private set; }
    public decimal Latitud { get; private set; }
    public decimal Longitud { get; private set; }
    public DateTimeOffset FechaHoraIncidente { get; private set; }
    public string UsuarioReportanteId { get; private set; } = null!;
    public EstadoRobo Estado { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Robo()
    {
        // EF Core
    }

    public static Robo Crear(
        TipoIncidente tipoIncidente,
        decimal latitud,
        decimal longitud,
        DateTimeOffset fechaHoraIncidente,
        string usuarioReportanteId,
        string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(usuarioReportanteId))
            throw new ArgumentException("El robo debe tener un usuario reportante.", nameof(usuarioReportanteId));

        return new Robo
        {
            Id = Guid.NewGuid(),
            TipoIncidente = tipoIncidente,
            Descripcion = descripcion,
            Latitud = latitud,
            Longitud = longitud,
            FechaHoraIncidente = fechaHoraIncidente,
            UsuarioReportanteId = usuarioReportanteId,
            Estado = EstadoRobo.Reportado,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
