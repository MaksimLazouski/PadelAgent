namespace PadelAgent.Dtos;

public class SearchCourtsResponse
{
    public List<DayCourtsDto> Dates { get; set; }
}

public class DayCourtsDto
{
    public DateTime Date { get; set; }
    public List<CourtDto> Courts { get; set; }
}

public class CourtDto
{
    public string CourtName { get; set; }
    public List<CourtSlotDto> Slots { get; set; }
}

public class CourtSlotDto
{
    public TimeOnly Start { get; set; }
    public List<CourtDurationDto> Durations { get; set; }
}

public class CourtDurationDto
{
    public int DurationMinutes { get; set; }
    public int PriceAmount { get; init; }
    public string PriceCurrency { get; init; }
}