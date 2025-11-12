using PadelAgent.Engine.Agents;
using PadelAgent.Engine.Agents.Interfaces;
using PadelAgent.Engine.Services;
using PadelAgent.Engine.Services.Interfaces;
using PadelAgent.Engine.Tools;
using PadelAgent.Engine.Tools.Interfaces;

namespace PadelAgent.Engine;

public static class DependencyInjection
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IClubMetadataService>(
            _ => ClubMetadataService.FromJsonFile("Resources/InterPadel.json"));
        services.AddSingleton<IPlaytomicAvailabilityService, PlaytomicAvailabilityService>();
        services.AddSingleton<ICalendarService, CalendarService>();
        services.AddHttpClient("CalendarClient");
        services.AddSingleton<IAgentToolsProvider, AgentToolsProvider>();
        services.AddSingleton<IAgentFactory, AgentFactory>();
    }
}