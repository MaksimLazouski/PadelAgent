using System.Text.Json;
using PadelAgent.Configurations;
using Refit;

namespace PadelAgent.Engine.Clients;

public static class DependencyInjection
{
    public static void AddPlaytomicClient(this IServiceCollection services, IConfiguration configuration, string configurationKey = "PlaytomicHttpClient")
    {
        var clientSettings = new HttpClientSettings();
        configuration.Bind(configurationKey, clientSettings);

        if (clientSettings == null)
        {
            throw new Exception(
                $"Unable to construct Playtomic client using provided configuration key: {configurationKey}");
        }

        var jsonOptions = new SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = true
        });

        var playtomicClientSettings = new HttpClientSettings();
        configuration.Bind("PlaytomicHttpClient", playtomicClientSettings);
        services
            .AddRefitClient<IPlaytomicClient>(new RefitSettings { ContentSerializer = jsonOptions })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(playtomicClientSettings.ApiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", "PadelAgent/1.0");
            });
    }
}