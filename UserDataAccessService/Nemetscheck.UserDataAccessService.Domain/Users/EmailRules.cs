using System.Net.Mail;

namespace Nemetscheck.UserDataAccessService.Domain.Users;

public static class EmailRules
{
    public const int MaxLength = 256;

    public static string Normalize(string email) => email.Trim().ToLowerInvariant();

    public static string? Validate(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email is required";

        var normalized = Normalize(email);

        if (normalized.Length > MaxLength)
            return $"Email must not exceed {MaxLength} characters";

        if (!MailAddress.TryCreate(normalized, out var address) || address.Address != normalized)
            return "Email has an invalid format";

        return null;
    }
}
