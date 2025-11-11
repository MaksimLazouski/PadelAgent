namespace PadelAgent.Engine.Tools.Models;

public class AvailableSlotDto
{
    public string CourtId { get; init; }
    public string Start { get; init; }
    public string End { get; init; }
    public int DurationMinutes { get; init; }
    public int Price { get; init; }
    public string Currency { get; init; }
    public string Type { get; init; }
}
