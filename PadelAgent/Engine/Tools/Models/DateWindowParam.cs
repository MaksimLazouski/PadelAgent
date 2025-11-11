using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PadelAgent.Engine.Tools.Models;

public class DateWindowParam
{
    [Description("Specific date (yyyy-MM-dd) this time window applies to.")]
    [JsonPropertyName("date")]
    public DateOnly Date { get; init; }

    [Description("Time window start for this date (HH:mm, optional).")]
    [JsonPropertyName("timeFrom")]
    public string TimeFrom { get; init; }

    [Description("Time window end for this date (HH:mm, optional).")]
    [JsonPropertyName("timeTo")]
    public string TimeTo { get; init; }
}
