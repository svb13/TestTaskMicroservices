using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nemetscheck.Shared.ErrorHandling;
using Nemetscheck.Shared.Validation;

namespace Nemetscheck.Shared;

public static class ApiServiceCollectionExtensions
{
    public static IMvcBuilder AddApiDefaults<TMarker>(this IServiceCollection services, Action<ExceptionMappingOptions> mapExceptions)
    {
        ArgumentNullException.ThrowIfNull(mapExceptions);

        services.AddValidatorsFromAssemblyContaining<TMarker>(includeInternalTypes: true);
        services.AddOpenApi();
        services.AddProblemDetails();
        services.Configure(mapExceptions);
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services.AddControllers(o => o.Filters.Add<FluentValidationFilter>());
    }
}
