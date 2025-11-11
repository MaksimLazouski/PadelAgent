using System.Text.Json;
using PadelAgent.Engine.Services.Interfaces;
using PadelAgent.Models;

namespace PadelAgent.Engine.Services;

public class ClubMetadataService : IClubMetadataService
{
    private readonly Dictionary<string, ClubMetadata> _clubs;

    public ClubMetadataService(IEnumerable<ClubMetadata> clubs)
    {
        _clubs = clubs.ToDictionary(c => c.ClubId, StringComparer.OrdinalIgnoreCase);
    }

    public static ClubMetadataService FromJsonFile(string path)
    {
        var text = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var clubs = text.TrimStart().StartsWith("[")
            ? JsonSerializer.Deserialize<List<ClubMetadata>>(text, options)
            : [JsonSerializer.Deserialize<ClubMetadata>(text, options)];
        return new ClubMetadataService(clubs.Where(c => c is not null));
    }

    public ClubMetadata GetClubByClubId(string clubId)
        => _clubs.GetValueOrDefault(clubId);

    public List<string> GetCourtIds(string clubId, CourtType? courtType = null)
    {
        var club = GetClubByClubId(clubId);
        if (club is null) return [];

        var courts = club.Courts;
        if (courtType is not null)
            courts = courts.Where(court => court.CourtType == courtType).ToList();

        return courts
            .Select(r => r.CourtId).ToList();
    }

    public CourtType GetCourtType(string clubId, string courtId)
    {
        var resourceMetadata = GetClubByClubId(clubId).Courts
            .First(r => r.CourtId.Equals(courtId, StringComparison.OrdinalIgnoreCase));
        return resourceMetadata.CourtType;
    }

    public string GetCourtName(string clubId, string courtId)
    {
        var resourceMetadata = GetClubByClubId(clubId).Courts
            .First(r => r.CourtId.Equals(courtId, StringComparison.OrdinalIgnoreCase));
        return resourceMetadata.Name;
    }
}
