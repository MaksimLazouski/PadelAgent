using System.Text.Json.Serialization;

namespace PadelAgent.Engine.Clients.Contracts;

public class PlaytomicSlotDto
{
    [JsonPropertyName("start_time")]
    public string StartTime { get; init; }

    [JsonPropertyName("duration")]
    public int DurationMinutes { get; init; }

    [JsonPropertyName("price")]
    public string PriceRaw { get; init; }
}
