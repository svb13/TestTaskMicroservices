using Microsoft.AspNetCore.Mvc;
using Nemetscheck.UserDataAccessService.Services.Auth;
using Nemetscheck.UserDataAccessService.Contracts.Responses;
using Nemetscheck.UserDataAccessService.Contracts.Requests;

namespace Nemetscheck.UserDataAccessService.Controllers;

[ApiController]
[Route("api/tokens")]
public sealed class TokensController(IAccessTokenService accessTokenService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IssueAsync(IssueTokenRequest request, CancellationToken cancellationToken)
    {
        var token = await accessTokenService.IssueAsync(
            new IssueAccessTokenCommand(request.Email, request.Password), cancellationToken);
        
        return Ok(token);
    }
}
