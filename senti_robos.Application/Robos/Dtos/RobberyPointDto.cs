namespace senti_robos.Application.Robos.Dtos;

// Mirrors senti_mobile's RobberyPointDto (data/remote/dto/RobberyPointDto.kt)
// field-for-field so the future gateway can reverse-proxy this response
// without translation.
public record RobberyPointDto(
    string Id,
    double Latitude,
    double Longitude,
    long? Timestamp,
    string? Type);
