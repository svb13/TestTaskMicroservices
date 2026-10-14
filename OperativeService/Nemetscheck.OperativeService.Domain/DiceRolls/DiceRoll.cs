namespace Nemetscheck.OperativeService.Domain.DiceRolls;

public sealed class DiceRoll
{
    public const int MinFace = 1;
    public const int MaxFace = 6;

    private DiceRoll()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public int FirstDie { get; private set; }

    public int SecondDie { get; private set; }

    // stored rather than computed so sorting by sum can use an index
    public int Sum { get; private set; }

    public DateTime RolledAtUtc { get; private set; }

    public static DiceRoll Create(long userId, int firstDie, int secondDie, DateTime rolledAtUtc)
    {
        if (userId <= 0)
            throw new DomainException("User id must be positive");

        EnsureFace(firstDie);
        EnsureFace(secondDie);

        if (rolledAtUtc.Kind != DateTimeKind.Utc)
            throw new DomainException("Roll time must be in UTC");

        return new DiceRoll
        {
            UserId = userId,
            FirstDie = firstDie,
            SecondDie = secondDie,
            Sum = firstDie + secondDie,
            RolledAtUtc = rolledAtUtc
        };
    }

    private static void EnsureFace(int value)
    {
        if (value is < MinFace or > MaxFace)
            throw new DomainException($"Die value must be between {MinFace} and {MaxFace}");
    }
}
