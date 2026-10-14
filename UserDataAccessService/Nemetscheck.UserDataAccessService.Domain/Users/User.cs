using Nemetscheck.UserDataAccessService.Domain.Common;
using Nemetscheck.UserDataAccessService.Domain.Exceptions;

namespace Nemetscheck.UserDataAccessService.Domain.Users;

public sealed class User : AggregateRoot<long>
{
    public const int NameMaxLength = PersonNameRules.MaxLength;
    public const int EmailMaxLength = EmailRules.MaxLength;

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }
    
    // algorithm, iterations and salt are embedded
    public string PasswordHash { get; private set; }

    public string? ImagePath { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public static User Register(string firstName, string lastName, string email, string passwordHash, DateTime createdAtUtc)
    {
        EnsurePasswordHash(passwordHash);

        return new User(
            NormalizeName(firstName, "First name"),
            NormalizeName(lastName, "Last name"),
            NormalizeEmail(email),
            passwordHash,
            createdAtUtc);
    }

    public static string NormalizeEmail(string? email)
    {
        if (EmailRules.Validate(email) is { } error)
            throw new DomainException(error);

        return EmailRules.Normalize(email!);
    }

    public void AttachImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new DomainException("Image path is required");

        ImagePath = imagePath;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        EnsurePasswordHash(passwordHash);
        PasswordHash = passwordHash;
    }

    // for EF Core for materialization
    private User()
    {
        FirstName = null!;
        LastName = null!;
        Email = null!;
        PasswordHash = null!;
    }

    private User(string firstName, string lastName, string email, string passwordHash, DateTime createdAtUtc)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
    }

    private static void EnsurePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required");
    }

    private static string NormalizeName(string? value, string field)
    {
        if (PersonNameRules.Validate(value, field) is { } error)
            throw new DomainException(error);

        return value!.Trim();
    }
}
