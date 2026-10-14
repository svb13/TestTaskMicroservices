namespace Nemetscheck.OperativeService.Contracts.Responses;

public sealed record DiceRollResponse(long Id, int FirstDie, int SecondDie, int Sum, DateTime RolledAtUtc);
