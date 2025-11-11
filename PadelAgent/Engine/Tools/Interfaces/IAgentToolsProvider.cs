using Microsoft.Extensions.AI;
using PadelAgent.Engine.Tools.Models;
using System.ComponentModel;

namespace PadelAgent.Engine.Tools.Interfaces;

public interface IAgentToolsProvider
{
    IEnumerable<AIFunction> BuildTools();

    Task<string> SearchPlaytomicAsync(
        List<DateWindowParam> dates,
        int? durationMinutes,
        string courtType, CancellationToken ct);
}
