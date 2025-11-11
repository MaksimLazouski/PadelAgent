using PadelAgent.Models;

namespace PadelAgent.Engine.Services.Interfaces;

public interface IClubMetadataService
{
    ClubMetadata GetClubByClubId(string clubId);
    List<string> GetCourtIds(string clubId, CourtType? courtType = null);
    CourtType GetCourtType(string clubId, string courtId);
    string GetCourtName(string clubId, string courtId);
}
