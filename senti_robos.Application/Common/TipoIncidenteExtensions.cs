using senti_robos.Domain;

namespace senti_robos.Application.Common;

// Single source of truth for the wire format of TipoIncidente. The mobile
// app (senti_mobile) hardcodes these exact snake_case strings in three
// separate files (RobberyTypeFilterDropdown.kt, ReportScreenContent.kt,
// RobberyRepositoryImpl.kt) with no shared enum on its side — this backend
// is the only place that validates them.
public static class TipoIncidenteExtensions
{
    private static readonly IReadOnlyDictionary<string, TipoIncidente> ByWireValue =
        new Dictionary<string, TipoIncidente>(StringComparer.Ordinal)
        {
            ["armed_robbery"] = TipoIncidente.ArmedRobbery,
            ["theft"] = TipoIncidente.Theft,
            ["burglary"] = TipoIncidente.Burglary
        };

    public static bool TryParseWireValue(string? value, out TipoIncidente tipoIncidente)
    {
        if (value is not null && ByWireValue.TryGetValue(value, out var parsed))
        {
            tipoIncidente = parsed;
            return true;
        }

        tipoIncidente = default;
        return false;
    }

    public static string ToWireValue(this TipoIncidente tipoIncidente) => tipoIncidente switch
    {
        TipoIncidente.ArmedRobbery => "armed_robbery",
        TipoIncidente.Theft => "theft",
        TipoIncidente.Burglary => "burglary",
        _ => throw new ArgumentOutOfRangeException(nameof(tipoIncidente), tipoIncidente, null)
    };
}
