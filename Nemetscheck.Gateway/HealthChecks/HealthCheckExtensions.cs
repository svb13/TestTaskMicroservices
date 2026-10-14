using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nemetscheck.Gateway.HealthChecks;

internal static class HealthCheckExtensions
{
    private const string ReadyTag = "ready";

    // Registers one readiness check per YARP cluster destination so /health/ready reflects the backends.
    public static IServiceCollection AddBackendHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(nameof(BackendHealthCheck), client => client.Timeout = TimeSpan.FromSeconds(5));

        var healthChecks = services.AddHealthChecks();

        foreach (var cluster in configuration.GetSection("ReverseProxy:Clusters").GetChildren())
        {
            var healthPath = cluster["HealthCheck:Active:Path"] ?? "/health";

            foreach (var destination in cluster.GetSection("Destinations").GetChildren())
            {
                var address = destination["Address"];
                if (string.IsNullOrWhiteSpace(address))
                    continue;

                var healthUri = new Uri(new Uri(address), healthPath);

                healthChecks.Add(new HealthCheckRegistration(
                    $"{cluster.Key}:{destination.Key}",
                    sp => new BackendHealthCheck(sp.GetRequiredService<IHttpClientFactory>(), healthUri),
                    HealthStatus.Unhealthy,
                    [ReadyTag]));
            }
        }

        return services;
    }

    public static WebApplication MapGatewayHealthChecks(this WebApplication app)
    {
        // Liveness: only the gateway process itself.
        app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });

        // Readiness: gateway plus every backend service.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(ReadyTag),
            ResponseWriter = WriteJsonAsync
        });

        return app;
    }

    private static Task WriteJsonAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration,
            entries = report.Entries.ToDictionary(
                e => e.Key,
                e => new { status = e.Value.Status.ToString(), description = e.Value.Description, duration = e.Value.Duration })
        };

        return JsonSerializer.SerializeAsync(context.Response.Body, payload, cancellationToken: context.RequestAborted);
    }
}
