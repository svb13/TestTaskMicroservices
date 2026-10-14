using System.Text.RegularExpressions;

namespace Nemetscheck.UserDataAccessService.Domain.Users;

public static partial class PersonNameRules
{
    public const int MaxLength = 100;

    public static string? Validate(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            return $"{field} is required";

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            return $"{field} must not exceed {MaxLength} characters";

        if (!AllowedCharacters().IsMatch(trimmed))
            return $"{field} may contain only letters, spaces, hyphens and apostrophes";

        if (!trimmed.Any(char.IsLetter))
            return $"{field} must contain at least one letter";

        return null;
    }

    [GeneratedRegex(@"^[\p{L}\p{M} '\u2019-]+$")]
    private static partial Regex AllowedCharacters();
}
