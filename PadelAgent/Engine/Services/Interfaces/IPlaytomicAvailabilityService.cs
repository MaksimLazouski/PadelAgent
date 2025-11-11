using PadelAgent.Models;

namespace PadelAgent.Engine.Services.Interfaces;

public interface IPlaytomicAvailabilityService
{
    public Task<List<PlaytomicSlot>> GetSlotsAsync(
        string clubId,
        string sportId,
        List<DayWindow> dayWindows,
        int? durationMinutes = null,
        CourtType? courtType = null,
        CancellationToken ct = default);
}
