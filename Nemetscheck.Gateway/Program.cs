using Nemetscheck.Gateway.HealthChecks;

namespace Nemetscheck.Gateway;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.AddBackendHealthChecks(builder.Configuration);

        var app = builder.Build();

        app.MapGatewayHealthChecks();
        app.MapReverseProxy();

        await app.RunAsync();
    }
}
