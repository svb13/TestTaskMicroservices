using System.Text.Json.Serialization;
using Nemetscheck.OperativeService.Authentication;
using Nemetscheck.OperativeService.Domain;
using Nemetscheck.OperativeService.Infrastructure;
using Nemetscheck.OperativeService.Services;
using Nemetscheck.Shared;

namespace Nemetscheck.OperativeService;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services
			.AddApiDefaults<Program>(errors => errors
				.Map<DomainException>(StatusCodes.Status400BadRequest, "Validation failed")
				.Map<UnauthorizedException>(StatusCodes.Status401Unauthorized, "Unauthorized"))
			.AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

		builder.Services.AddJwtAuthentication(builder.Configuration);
		builder.Services.AddScoped<IDiceRollService, DiceRollService>();
		builder.Services.AddInfrastructure(builder.Configuration);
		builder.Services.AddHealthChecks();

		var app = builder.Build();

		await app.Services.ApplyMigrationsAsync();

		app.UseExceptionHandler();
		app.UseStatusCodePages();

		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
		}

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapHealthChecks("/health");
		app.MapControllers();

		await app.RunAsync();
	}
}
