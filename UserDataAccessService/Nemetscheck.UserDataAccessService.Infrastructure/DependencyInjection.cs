using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nemetscheck.UserDataAccessService.Domain.Abstractions;
using Nemetscheck.UserDataAccessService.Domain.Users;
using Nemetscheck.UserDataAccessService.Infrastructure.Persistence;
using Nemetscheck.UserDataAccessService.Infrastructure.Security;
using Nemetscheck.UserDataAccessService.Infrastructure.Storage;

namespace Nemetscheck.UserDataAccessService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentRootPath);

        var connectionString = configuration.GetConnectionString("UsersDb")
            ?? throw new InvalidOperationException("Connection string 'UsersDb' is not configured");

        services.AddDbContext<UsersDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();

        services.Configure<PasswordHasherOptions>(o =>
        {
            o.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
            o.IterationCount = 100_000;
        });
        services.AddSingleton<Domain.Abstractions.IPasswordHasher, IdentityPasswordHasher>();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        services.AddOptions<ImageStorageOptions>()
            .Bind(configuration.GetSection(ImageStorageOptions.SectionName))
            .ValidateDataAnnotations()
            .PostConfigure(o => o.RootPath = Path.GetFullPath(o.RootPath, contentRootPath))
            .ValidateOnStart();
        services.AddSingleton<IImageStorage, LocalImageStorage>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }
}
