using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nemetscheck.Shared.ErrorHandling;

internal sealed class GlobalExceptionHandler(
	IProblemDetailsService problemDetailsService,
	IOptions<ExceptionMappingOptions> options,
	ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
		CancellationToken cancellationToken)
	{
		var (status, title) = options.Value.Find(exception)
			?? (StatusCodes.Status500InternalServerError, "Unexpected error");

		if (status == StatusCodes.Status500InternalServerError)
			logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
		else
			logger.LogInformation("Request {Method} {Path} failed with {Status}: {Message}", httpContext.Request.Method, httpContext.Request.Path, status, exception.Message);

		httpContext.Response.StatusCode = status;

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = CreateProblemDetails(status, title, exception)
		});
	}

	// 400s use the same shape as FluentValidation/model binding errors so clients handle a single format;
	// domain errors are not tied to a field, so they are reported as model-level errors (empty key)
	private static ProblemDetails CreateProblemDetails(int status, string title, Exception exception) =>
		status switch
		{
			StatusCodes.Status400BadRequest => new ValidationProblemDetails(
				new Dictionary<string, string[]> { [string.Empty] = [exception.Message] })
			{
				Status = status
			},
			StatusCodes.Status500InternalServerError => new ProblemDetails
			{
				Status = status,
				Title = title,
				Detail = "An unexpected error occurred"
			},
			_ => new ProblemDetails
			{
				Status = status,
				Title = title,
				Detail = exception.Message
			}
		};
}
