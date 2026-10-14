using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Nemetscheck.UserDataAccessService.Domain.Abstractions;

namespace Nemetscheck.UserDataAccessService.Infrastructure.Security;

internal sealed class IdentityPasswordHasher(IOptions<PasswordHasherOptions> options) : Domain.Abstractions.IPasswordHasher
{
    private static readonly object HashUser = new();

    private readonly PasswordHasher<object> _hasher = new(options);

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        return _hasher.HashPassword(HashUser, Normalize(password));
    }

    public PasswordVerification Verify(string passwordHash, string providedPassword)
    {
        if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(providedPassword))
            return PasswordVerification.Failed;

        string normalized;
        try
        {
            normalized = Normalize(providedPassword);
        }
        catch (ArgumentException)
        {
            return PasswordVerification.Failed;
        }

        return _hasher.VerifyHashedPassword(HashUser, passwordHash, normalized) switch
        {
            PasswordVerificationResult.Success => PasswordVerification.Success,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordVerification.RehashNeeded,
            _ => PasswordVerification.Failed
        };
    }

    private static string Normalize(string password) => password.Normalize(NormalizationForm.FormKC);
}
