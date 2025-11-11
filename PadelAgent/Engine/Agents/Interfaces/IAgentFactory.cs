using Microsoft.Agents.AI;

namespace PadelAgent.Engine.Agents.Interfaces;

public interface IAgentFactory
{
    ChatClientAgent CreatePadelAgent();
}
