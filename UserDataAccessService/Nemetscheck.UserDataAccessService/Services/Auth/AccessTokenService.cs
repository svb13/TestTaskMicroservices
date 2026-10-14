using Nemetscheck.UserDataAccessService.Contracts.Responses;
using Nemetscheck.UserDataAccessService.Domain.Abstractions;
using Nemetscheck.UserDataAccessService.Domain.Exceptions;
using Nemetscheck.UserDataAccessService.Domain.Users;

namespace Nemetscheck.UserDataAccessService.Services.Auth;

internal sealed class AccessTokenService(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator) : IAccessTokenService
{
    private const string InvalidCredentials = "Invalid email or password";

    public async Task<AccessTokenResponse> IssueAsync(IssueAccessTokenCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        string email;
        
        try
        {
            email = User.NormalizeEmail(command.Email);
        }
        catch (DomainException)
        {
            throw new UnauthorizedException(InvalidCredentials);
        }

        var user = await users.GetByEmailAsync(email, cancellationToken)
            ?? throw new UnauthorizedException(InvalidCredentials);

        var verification = passwordHasher.Verify(user.PasswordHash, command.Password);
        
        if (verification == PasswordVerification.Failed)
            throw new UnauthorizedException(InvalidCredentials);

        if (verification == PasswordVerification.RehashNeeded)
        {
            user.ChangePasswordHash(passwordHasher.Hash(command.Password));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var token = tokenGenerator.Generate(user);
        
        return new AccessTokenResponse(token.Value, "Bearer", token.ExpiresAtUtc);
    }
}
