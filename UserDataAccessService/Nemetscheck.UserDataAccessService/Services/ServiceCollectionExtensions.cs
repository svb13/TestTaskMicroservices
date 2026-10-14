using Nemetscheck.UserDataAccessService.Services.Auth;
using Nemetscheck.UserDataAccessService.Services.Users;

namespace Nemetscheck.UserDataAccessService.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        return services;
    }
}
