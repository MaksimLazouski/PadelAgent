using Microsoft.AspNetCore.Mvc;
using PadelAgent.Dtos;
using PadelAgent.Engine.Tools.Interfaces;

namespace PadelAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaytomicController(IAgentToolsProvider agentToolsProvider) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] SearchCourtsRequest request, CancellationToken cancellationToken = default)
        {
            var response =
                await agentToolsProvider.SearchPlaytomicAsync(request.Dates, request.DurationMinutes, request.CourtType,
                    cancellationToken);
            return Ok(response);
        }
    }
}
