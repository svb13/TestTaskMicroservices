using Microsoft.AspNetCore.Mvc;
using Nemetscheck.UserDataAccessService.Services.Users;
using Nemetscheck.UserDataAccessService.Contracts.Responses;
using Nemetscheck.UserDataAccessService.Contracts.Requests;

namespace Nemetscheck.UserDataAccessService.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserRegistrationService registrationService) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromForm] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        await using var imageStream = request.Image?.OpenReadStream();

        var image = request.Image is { } file && imageStream is not null
            ? new ImageUpload(imageStream, file.FileName, file.Length)
            : null;

        var user = await registrationService.RegisterAsync(
            new RegisterUserCommand(request.FirstName, request.LastName, request.Email, request.Password, image),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, user);
    }
}
