using Microsoft.AspNetCore.Mvc;
using PadelAgent.Dtos;
using PadelAgent.Engine.Services.Interfaces;
using PadelAgent.Engine.Tools.Interfaces;

namespace PadelAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaytomicController(IAgentToolsProvider agentToolsProvider, ICalendarService calendarService) : ControllerBase
    {
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] SearchCourtsRequest request, CancellationToken cancellationToken = default)
        {
            var playtomicSlots =
                await agentToolsProvider.SearchPlaytomicAsync(request.Dates, request.DurationMinutes, request.CourtType,
                    cancellationToken);

            var response = new SearchCourtsResponse();
            var byDate = playtomicSlots
                .GroupBy(s => s.Start.Date)
                .OrderBy(g => g.Key);
            foreach (var dayGroup in byDate)
            {
                var dayCourts = new DayCourtsDto
                {
                    Date = dayGroup.Key.Date,
                    Courts = []
                };
                var courts = new List<CourtDto>();

                var byCourt = dayGroup
                    .GroupBy(s => s.CourtId)
                    .OrderBy(g => g.First().CourtName);

                foreach (var courtGroup in byCourt)
                {
                    var court = new CourtDto
                    {
                        CourtName = courtGroup.First().CourtName,
                    };
                    var slots = new List<CourtSlotDto>();
                    var byStartTime = courtGroup
                        .GroupBy(s => s.Start.TimeOfDay)
                        .OrderBy(g => g.Key);

                    foreach (var startGroup in byStartTime)
                    {
                        var courtSlot = new CourtSlotDto
                        {
                            Start = new TimeOnly(
                                startGroup.First().Start.Hour,
                                startGroup.First().Start.Minute)
                        };

                        var durations = startGroup
                            .OrderBy(x => x.DurationMinutes)
                            .Select(x => new CourtDurationDto
                            {
                                DurationMinutes = x.DurationMinutes,
                                PriceAmount = x.PriceAmount,
                                PriceCurrency = x.PriceCurrency
                            }).ToList();
                        courtSlot.Durations = durations;
                        slots.Add(courtSlot);
                    }

                    court.Slots = slots;
                    courts.Add(court);
                }

                dayCourts.Courts = courts;
            }

            return Ok(response);
        }

        [HttpPost("calendar")]
        public async Task<IActionResult> GetCalendarEvents([FromBody] GetCalendarEventsRequest request, CancellationToken cancellationToken = default)
        {
            var response =
                await calendarService.GetEventsAsync(request.From, request.To, cancellationToken);
            return Ok(response);
        }
    }
}
