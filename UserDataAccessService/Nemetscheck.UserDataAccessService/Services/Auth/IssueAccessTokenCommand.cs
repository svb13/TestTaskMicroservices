namespace Nemetscheck.UserDataAccessService.Services.Auth;

public sealed record IssueAccessTokenCommand(string Email, string Password);
