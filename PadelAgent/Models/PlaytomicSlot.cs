namespace PadelAgent.Models;

public class PlaytomicSlot
{
    public string ClubId { get; init; }
    public string CourtId { get; init; }
    public CourtType Type { get; init; }
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int DurationMinutes { get; init; }
    public int PriceAmount { get; init; }
    public string PriceCurrency { get; init; }
}
