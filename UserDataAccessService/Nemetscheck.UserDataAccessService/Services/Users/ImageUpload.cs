namespace Nemetscheck.UserDataAccessService.Services.Users;

public sealed record ImageUpload(Stream Content, string FileName, long Length);
