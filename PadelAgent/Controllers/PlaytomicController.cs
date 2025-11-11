using Microsoft.AspNetCore.Mvc;
using PadelAgent.Engine.Agents.Interfaces;

namespace PadelAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaytomicController(IAgentFactory agentFactory) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] string searchText)
        {
            var agent = agentFactory.CreatePadelAgent();
            var response = await agent.RunAsync(searchText);
            return Ok(response.Messages.Last().Text);
        }
    }
}
