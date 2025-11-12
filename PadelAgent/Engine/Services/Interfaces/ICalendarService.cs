using PadelAgent.Models;

namespace PadelAgent.Engine.Services.Interfaces;

public interface ICalendarService
{
    Task<List<CalendarEventInfo>> GetEventsAsync(
        DateTime from, DateTime to, CancellationToken ct = default);
}