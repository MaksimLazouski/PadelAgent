using System.Collections.Concurrent;
using System.Globalization;
using PadelAgent.Engine.Clients;
using PadelAgent.Engine.Services.Interfaces;
using PadelAgent.Models;

namespace PadelAgent.Engine.Services;

public class PlaytomicAvailabilityService(IPlaytomicClient playtomicClient,
    IClubMetadataService clubMetadataService) : IPlaytomicAvailabilityService
{
    private const int MaxParallelRequestsCount = 4;

    public async Task<List<PlaytomicSlot>> GetSlotsAsync(string clubId, string sportId, List<DayWindow> dayWindows, int? durationMinutes = null,
        CourtType? courtType = null, CancellationToken ct = default)
    {
        try
        {
            if (dayWindows.Count == 0) return [];

            var allowedCourtIds = clubMetadataService.GetCourtIds(clubId, courtType);
            if (allowedCourtIds.Count == 0)
                return [];

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxParallelRequestsCount,
                CancellationToken = ct
            };

            var playtomicSlots = new ConcurrentBag<PlaytomicSlot>();
            await Parallel.ForEachAsync(dayWindows, parallelOptions,
                async (dayWindow, token) =>
                {
                    var date = dayWindow.Date;
                    var (allowedFrom, allowedTo) = (dayWindow.From, dayWindow.To);
                    var dateIso = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                    var availableCourts = await playtomicClient.GetAvailableSlotsAsync(clubId, dateIso, sportId,
                        token);
                    if (availableCourts is null || availableCourts.Count == 0)
                        return;

                    foreach (var availableCourt in availableCourts)
                    {
                        if (string.IsNullOrWhiteSpace(availableCourt.CourtId))
                            continue;
                        if (!allowedCourtIds.Contains(availableCourt.CourtId))
                            continue;
                        if (availableCourt.Slots is null || availableCourt.Slots.Count == 0)
                            continue;
                        var type = clubMetadataService.GetCourtType(clubId, availableCourt.CourtId);
                        var courtName = clubMetadataService.GetCourtName(clubId, availableCourt.CourtId);
                        foreach (var availableSlot in availableCourt.Slots)
                        {
                            if (durationMinutes.HasValue && availableSlot.DurationMinutes != durationMinutes.Value)
                                continue;

                            var (amount, currency) = ParsePrice(availableSlot.PriceRaw);
                            var time = TimeOnly.ParseExact(availableSlot.StartTime, "HH:mm:ss",
                                CultureInfo.InvariantCulture);
                            if (time < allowedFrom || time > allowedTo)
                                continue;

                            var start = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second,
                                DateTimeKind.Unspecified);

                            playtomicSlots.Add(new PlaytomicSlot
                            {
                                ClubId = clubId,
                                CourtId = availableCourt.CourtId,
                                CourtName = courtName,
                                DurationMinutes = availableSlot.DurationMinutes,
                                Start = start,
                                End = start.AddMinutes(availableSlot.DurationMinutes),
                                Type = type,
                                PriceAmount = amount,
                                PriceCurrency = currency
                            });
                        }

                    }
                });

            var result = playtomicSlots
                .GroupBy(x => (x.ClubId, x.CourtId, x.Start, x.DurationMinutes))
                .Select(g => g.First())
                .OrderBy(x => x.Start)
                .ToList();

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static (int amount, string currency) ParsePrice(string raw)
    {
        var parts = raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var amount = int.Parse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture);

        var currency = parts[1];

        return (amount, currency);
    }
}
