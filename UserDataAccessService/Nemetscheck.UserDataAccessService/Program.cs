using Nemetscheck.Shared;
using Nemetscheck.UserDataAccessService.Domain.Exceptions;
using Nemetscheck.UserDataAccessService.Infrastructure;
using Nemetscheck.UserDataAccessService.Services;

namespace Nemetscheck.UserDataAccessService;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddApiDefaults<Program>(errors => errors
			.Map<DomainException>(StatusCodes.Status400BadRequest, "Validation failed")
			.Map<ValidationException>(StatusCodes.Status400BadRequest, "Validation failed")
			.Map<UnauthorizedException>(StatusCodes.Status401Unauthorized, "Unauthorized")
			.Map<ConflictException>(StatusCodes.Status409Conflict, "Conflict"));

		builder.Services.AddUseCases();
		builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.ContentRootPath);
		builder.Services.AddHealthChecks();

		var app = builder.Build();

		await app.Services.ApplyMigrationsAsync();

		app.UseExceptionHandler();
		app.UseStatusCodePages();

		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
		}

		app.MapHealthChecks("/health");
		app.MapControllers();

		await app.RunAsync();
	}
}
