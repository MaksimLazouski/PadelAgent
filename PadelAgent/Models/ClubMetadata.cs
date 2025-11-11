using System.Text.Json.Serialization;

namespace PadelAgent.Models;

public class ClubMetadata
{
    public string ClubId { get; set; }
    public string Name { get; set; }
    public string SportId { get; set; }
    public List<CourtMetadata> Courts { get; set; }
}

public class CourtMetadata
{
    public string CourtId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }

    [JsonIgnore] public CourtType CourtType => Enum.Parse<CourtType>(Type, ignoreCase: true);
}
