using System.Text.Json.Serialization;

namespace PadelAgent.Engine.Clients.Contracts;

public class PlaytomicCourtDto
{
    [JsonPropertyName("resource_id")]
    public string CourtId { get; init; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; init; }

    [JsonPropertyName("slots")]
    public List<PlaytomicSlotDto> Slots { get; init; } = [];
}
