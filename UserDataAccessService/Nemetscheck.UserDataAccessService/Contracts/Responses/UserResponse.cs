namespace Nemetscheck.UserDataAccessService.Contracts.Responses;

public sealed record UserResponse(long Id, string FirstName, string LastName, string Email, string? ImagePath, DateTime CreatedAtUtc);
