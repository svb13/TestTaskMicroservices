using Nemetscheck.UserDataAccessService.Contracts.Responses;

namespace Nemetscheck.UserDataAccessService.Services.Users;

public interface IUserRegistrationService
{
    Task<UserResponse> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken);
}
