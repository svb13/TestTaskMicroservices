namespace Nemetscheck.UserDataAccessService.Contracts.Requests;

public sealed record RegisterUserRequest
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public IFormFile? Image { get; init; }
}
