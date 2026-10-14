using System.Globalization;
using System.Text;

namespace Nemetscheck.UserDataAccessService.Domain.Users;

public static class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 128;

    public static string? Validate(string? password)
    {
        if (string.IsNullOrEmpty(password))
            return "Password is required";

        string normalized;
        
        try
        {
            normalized = password.Normalize(NormalizationForm.FormKC);
        }
        catch (ArgumentException)
        {
            return "Password contains invalid Unicode characters";
        }

        if (string.IsNullOrWhiteSpace(normalized))
            return "Password must not consist only of whitespace";

        var length = 0;
        foreach (var rune in normalized.EnumerateRunes())
        {
            if (Rune.GetUnicodeCategory(rune) is UnicodeCategory.Control or UnicodeCategory.Format
                or UnicodeCategory.Surrogate or UnicodeCategory.OtherNotAssigned)
                return "Password contains unsupported control or unassigned characters";

            length++;
        }

        return length is < MinLength or > MaxLength
            ? $"Password must be between {MinLength} and {MaxLength} characters long"
            : null;
    }
}
