namespace Nemetscheck.UserDataAccessService.Contracts.Responses;

public sealed record AccessTokenResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);
