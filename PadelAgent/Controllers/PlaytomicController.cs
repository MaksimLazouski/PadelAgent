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
            var response =
                await agentToolsProvider.SearchPlaytomicAsync(request.Dates, request.DurationMinutes, request.CourtType,
                    cancellationToken);
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
