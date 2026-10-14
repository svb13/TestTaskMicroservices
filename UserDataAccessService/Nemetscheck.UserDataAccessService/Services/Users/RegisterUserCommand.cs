namespace Nemetscheck.UserDataAccessService.Services.Users;

public sealed record RegisterUserCommand(string FirstName, string LastName, string Email,
    string Password, ImageUpload? Image);
