using PadelAgent.Engine.Tools.Models;

namespace PadelAgent.Dtos;

public class SearchCourtsRequest
{
    public List<DateWindowParam> Dates { get; set; }
    public int? DurationMinutes { get; set; }
    public string CourtType { get; set; }
}
