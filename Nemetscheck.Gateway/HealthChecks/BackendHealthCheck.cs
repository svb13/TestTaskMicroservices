using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nemetscheck.Gateway.HealthChecks;

internal sealed class BackendHealthCheck(IHttpClientFactory httpClientFactory, Uri healthUri) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = httpClientFactory.CreateClient(nameof(BackendHealthCheck));
            using var response = await client.GetAsync(healthUri, cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"{healthUri} returned {(int)response.StatusCode}")
                : new HealthCheckResult(context.Registration.FailureStatus, $"{healthUri} returned {(int)response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, $"{healthUri} is unreachable", ex);
        }
    }
}
