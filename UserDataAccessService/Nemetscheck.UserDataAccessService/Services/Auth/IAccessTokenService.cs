using Nemetscheck.UserDataAccessService.Contracts.Responses;

namespace Nemetscheck.UserDataAccessService.Services.Auth;

public interface IAccessTokenService
{
    Task<AccessTokenResponse> IssueAsync(IssueAccessTokenCommand command, CancellationToken cancellationToken);
}
