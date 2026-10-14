using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nemetscheck.OperativeService.Domain;
using Nemetscheck.OperativeService.Domain.DiceRolls;
using Nemetscheck.OperativeService.Infrastructure.Dice;
using Nemetscheck.OperativeService.Infrastructure.Persistence;

namespace Nemetscheck.OperativeService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("OperativeDb")
            ?? throw new InvalidOperationException("Connection string 'OperativeDb' is not configured");

        services.AddDbContext<OperativeDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OperativeDbContext>());
        services.AddScoped<IDiceRollRepository, DiceRollRepository>();
        services.AddSingleton<IDiceRoller, RandomDiceRoller>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OperativeDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }
}
