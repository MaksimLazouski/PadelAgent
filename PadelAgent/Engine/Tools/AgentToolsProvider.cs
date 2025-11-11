using Microsoft.Extensions.AI;
using PadelAgent.Engine.Services.Interfaces;
using PadelAgent.Engine.Tools.Interfaces;
using PadelAgent.Engine.Tools.Models;
using PadelAgent.Models;
using System.ComponentModel;
using System.Globalization;

namespace PadelAgent.Engine.Tools;

public class AgentToolsProvider(IPlaytomicAvailabilityService playtomicAvailabilityService, IClubMetadataService clubMetadataService) : IAgentToolsProvider
{
    private const string DefaultClubId = "057c5f40-f54b-4e4d-977c-1f9547a25076";
    private readonly (TimeOnly from, TimeOnly to) _weekdayDefaultTime = new(new TimeOnly(15, 0), new TimeOnly(20, 0));
    private readonly (TimeOnly from, TimeOnly to) _weekendDefaultTime = new(new TimeOnly(9, 0), new TimeOnly(20, 0));
    public IEnumerable<AIFunction> BuildTools()
    {
        yield return AIFunctionFactory.Create(
            SearchPlaytomicAsync,
            new AIFunctionFactoryOptions
            {
                Name = "search_and_format_playtomic_slots",
                Description =
                    "Find available PADEL slots at playtomic and return a ready-to-send formatted text.\n" +
                    "INPUT FORMAT:\n" +
                    "  dates: [\n" +
                    "    { date: 'yyyy-MM-dd', timeFrom?: 'HH:mm', timeTo?: 'HH:mm' }, ...\n" +
                    "  • timeFrom/timeTo are OPTIONAL. \n" +
                    "  ]\n" +
                    "  • Each date may have its own optional time window.\n" +
                    "FILTERS (optional):\n" +
                    "  • durationMinutes (e.g. 60, 90, 120)\n" +
                    "  • courtType: 'Single' | 'Double'\n" +
                    "OUTPUT: A ready-to-send text summary (Markdown/plain)."
            }
        );
    }

    [Description(
        "Search PADEL court availability at Interpadel using a per-date list.\n" +
        "Each item may include its own timeFrom/timeTo window (both optional).\n" +
        "Returns a ready-to-send formatted text summary."
    )]
    public async Task<string> SearchPlaytomicAsync(
        [Description("Explicit list of dates (yyyy-MM-dd) with optional per-date windows.")]
        List<DateWindowParam> dates,
        [Description("Desired slot duration in minutes (optional). If null, return all durations.")]
        int? durationMinutes,
        [Description("Court type filter: 'Single' or 'Double' (optional).")]
        string courtType, CancellationToken ct)
    {
        var clubMetadata = clubMetadataService.GetClubByClubId(DefaultClubId);

        CourtType? courtTypeValue = null;
        if (!string.IsNullOrEmpty(courtType))
            courtTypeValue = Enum.Parse<CourtType>(courtType, ignoreCase: true);
        var dayWindows = BuildDayWindows(dates);

        var slots = await playtomicAvailabilityService.GetSlotsAsync(clubId: clubMetadata.ClubId,
            sportId: clubMetadata.SportId,
            dayWindows: dayWindows,
            durationMinutes: durationMinutes,
            courtType: courtTypeValue,
            ct: ct);

        return FormatSlots(slots);
    }

    public string FormatSlots(List<PlaytomicSlot> slots)
    {
        try
        {
            if (slots is null || slots.Count == 0)
                return "No available slots.";

            var sb = new System.Text.StringBuilder();

            var byDate = slots
                .GroupBy(s => s.Start.Date)
                .OrderBy(g => g.Key);

            foreach (var dayGroup in byDate)
            {
                sb.AppendLine($"{dayGroup.Key:dd MMMM (dddd, yyyy)}");

                var byCourt = dayGroup
                    .GroupBy(s => s.CourtId)
                    .OrderBy(g =>
                    {
                        var any = g.First();
                        var name = clubMetadataService.GetCourtName(any.ClubId, any.CourtId);
                        return name;
                    });

                foreach (var courtGroup in byCourt)
                {
                    var any = courtGroup.First();
                    var courtName = clubMetadataService.GetCourtName(any.ClubId, any.CourtId);
                    if (string.IsNullOrWhiteSpace(courtName))
                        courtName = any.CourtId;

                    sb.AppendLine($"{courtName} ({any.Type})");

                    var byStartTime = courtGroup
                        .GroupBy(s => s.Start.TimeOfDay)
                        .OrderBy(g => g.Key);

                    foreach (var startGroup in byStartTime)
                    {
                        var time = new TimeOnly(
                            startGroup.First().Start.Hour,
                            startGroup.First().Start.Minute);

                        var variants = startGroup
                            .OrderBy(x => x.DurationMinutes)
                            .Select(x => $"{x.DurationMinutes} min: {x.PriceAmount} {x.PriceCurrency}");

                        sb.AppendLine($"{time:HH\\:mm} — {string.Join("; ", variants)}");
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString().TrimEnd();
        }
        catch
        {
            return "Exception during formating slots result";
        }
    }

    private static TimeOnly? ParseTime(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        input = input.Trim();

        string[] formats = ["HH:mm", "H:mm", "HH", "H", "HH.mm", "H.mm"];

        foreach (var fmt in formats)
        {
            if (TimeOnly.TryParseExact(input, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;
        }

        if (TimeOnly.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            return parsed;

        return null;
    }


    private List<DayWindow> BuildDayWindows(List<DateWindowParam> dates)
    {
        return dates
            .Select(x =>
            {
                var from = ParseTime(x.TimeFrom);
                var to = ParseTime(x.TimeTo);
                var (f, t) = ResolvePerDateOrDefault(x.Date, from, to);
                return new DayWindow(x.Date, f, t);
            })
            .Distinct()
            .OrderBy(d => d.Date)
            .ToList();
    }

    private (TimeOnly from, TimeOnly to) ResolvePerDateOrDefault(
        DateOnly date, TimeOnly? perDateFrom, TimeOnly? perDateTo)
    {
        if (perDateFrom is not null && perDateTo is not null)
            return NormalizeTime(perDateFrom.Value, perDateTo.Value);

        var isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        return isWeekend
            ? _weekendDefaultTime
            : _weekdayDefaultTime;
    }

    private static (TimeOnly from, TimeOnly to) NormalizeTime(TimeOnly a, TimeOnly b)
        => a <= b ? (a, b) : (b, a);
}
