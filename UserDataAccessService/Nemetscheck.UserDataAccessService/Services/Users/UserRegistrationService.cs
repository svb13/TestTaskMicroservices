using Nemetscheck.UserDataAccessService.Contracts.Responses;
using Nemetscheck.UserDataAccessService.Domain.Abstractions;
using Nemetscheck.UserDataAccessService.Domain.Exceptions;
using Nemetscheck.UserDataAccessService.Domain.Users;

namespace Nemetscheck.UserDataAccessService.Services.Users;

internal sealed class UserRegistrationService(
    IUserRepository usersRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IImageStorage imageStorage,
    TimeProvider timeProvider,
    ILogger<UserRegistrationService> logger) : IUserRegistrationService
{
    public async Task<UserResponse> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        // the aggregate only receives the hash, so the password policy is enforced before hashing
        if (PasswordPolicy.Validate(command.Password) is { } passwordError)
            throw new DomainException(passwordError);

        // checked before hashing so duplicate registrations do not pay for the expensive hash
        var email = User.NormalizeEmail(command.Email);

        if (await usersRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException($"A user with email '{email}' already exists");

        var user = User.Register(
            command.FirstName,
            command.LastName,
            command.Email,
            passwordHasher.Hash(command.Password),
            timeProvider.GetUtcNow().UtcDateTime);

        string? imagePath = null;
        
        if (command.Image is { } image)
        {
            imagePath = await imageStorage.SaveAsync(image.Content, image.FileName, image.Length, cancellationToken);
            user.AttachImage(imagePath);
        }

        try
        {
            await usersRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch when (imagePath is not null)
        {
            try
            {
                await imageStorage.DeleteAsync(imagePath, CancellationToken.None);
            }
            catch (IOException cleanupError)
            {
                logger.LogError(cleanupError, "Failed to delete orphaned image {ImagePath}", imagePath);
            }
            catch (UnauthorizedAccessException cleanupError)
            {
                logger.LogError(cleanupError, "Failed to delete orphaned image {ImagePath}", imagePath);
            }

            throw;
        }

        return new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.ImagePath, user.CreatedAtUtc);
    }
}
