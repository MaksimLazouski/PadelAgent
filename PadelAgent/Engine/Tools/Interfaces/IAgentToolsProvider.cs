using Microsoft.Extensions.AI;
using PadelAgent.Engine.Tools.Models;
using PadelAgent.Models;
using System.ComponentModel;

namespace PadelAgent.Engine.Tools.Interfaces;

public interface IAgentToolsProvider
{
    IEnumerable<AIFunction> BuildTools();

    Task<List<PlaytomicSlot>> SearchPlaytomicAsync(
        List<DateWindowParam> dates,
        int? durationMinutes,
        string courtType, CancellationToken ct);
}
