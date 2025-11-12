using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.Extensions.Options;
using PadelAgent.Configurations;
using PadelAgent.Engine.Services.Interfaces;
using PadelAgent.Models;

namespace PadelAgent.Engine.Services;

public class CalendarService(IHttpClientFactory httpClientFactory, IOptions<CalendarSettings> calendarSettings) : ICalendarService
{
    private string _icsCached;
    private Calendar _calendarCached;
    private DateTime _calendarFetchedAt;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("CalendarClient");

    public async Task<List<CalendarEventInfo>> GetEventsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(calendarSettings.Value.DefaultTimeZone);
        var cal = await GetOrRefreshCalendarAsync(ct);

        var start = new CalDateTime(from, timeZone.Id);
        var end = new CalDateTime(to, timeZone.Id);
        var occurrences = cal.GetOccurrences(start)
            .TakeWhileBefore(end);

        var result = new List<CalendarEventInfo>();
        foreach (var occ in occurrences)
        {
            var calendarEvent = (CalendarEvent)occ.Source;
            var period = occ.Period;

            var startUtc = TimeZoneInfo.ConvertTime(period.StartTime.AsUtc, timeZone);
            var endUtc = TimeZoneInfo.ConvertTime(period.EndTime?.AsUtc ?? period.EffectiveEndTime!.AsUtc, timeZone);

            result.Add(new CalendarEventInfo
            {
                Start = startUtc,
                End = endUtc,
                Title = calendarEvent.Summary ?? ""
            });
        }
        
        return result
            .Where(x => x.End > from && x.Start < to)
            .OrderBy(x => x.Start)
            .ToList();
    }

    private async Task<Calendar> GetOrRefreshCalendarAsync(CancellationToken ct)
    {
        var ttlAlive = DateTime.UtcNow - _calendarFetchedAt <
                       TimeSpan.FromMinutes(calendarSettings.Value.CacheMinutes);

        if (ttlAlive && _calendarCached is not null)
            return _calendarCached;

        using var request = new HttpRequestMessage(HttpMethod.Get, calendarSettings.Value.IcsUrl);

        using var resp = await _httpClient.SendAsync(request, ct);
        resp.EnsureSuccessStatusCode();

        _icsCached = await resp.Content.ReadAsStringAsync(ct);

        _calendarFetchedAt = DateTime.UtcNow;
        _calendarCached = Calendar.Load(_icsCached);

        return _calendarCached;
    }
}
