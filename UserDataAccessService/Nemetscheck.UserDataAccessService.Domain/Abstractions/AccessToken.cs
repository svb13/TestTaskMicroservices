namespace Nemetscheck.UserDataAccessService.Domain.Abstractions;

public sealed record AccessToken(string Value, DateTime ExpiresAtUtc);
