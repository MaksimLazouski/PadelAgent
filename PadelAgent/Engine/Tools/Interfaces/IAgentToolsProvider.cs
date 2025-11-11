using Microsoft.Extensions.AI;

namespace PadelAgent.Engine.Tools.Interfaces;

public interface IAgentToolsProvider
{
    IEnumerable<AIFunction> BuildTools();
}
