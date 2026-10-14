namespace Nemetscheck.UserDataAccessService.Contracts.Requests;

public sealed record IssueTokenRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
