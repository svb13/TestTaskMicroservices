namespace Nemetscheck.UserDataAccessService.Domain.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);

    PasswordVerification Verify(string passwordHash, string providedPassword);
}
