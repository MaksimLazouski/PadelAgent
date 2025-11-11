using PadelAgent.Engine.Clients.Contracts;
using Refit;

namespace PadelAgent.Engine.Clients;

public interface IPlaytomicClient
{
    [Get("/api/clubs/availability")]
    Task<List<PlaytomicCourtDto>> GetAvailableSlotsAsync(
        [AliasAs("tenant_id")] string clubId,
        [AliasAs("date")] string dateIso,
        [AliasAs("sport_id")] string sportId,
        CancellationToken ct = default);
}
