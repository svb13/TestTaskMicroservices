using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Contracts.Requests;

public sealed record GetDiceRollsRequest
{
    public int? Year { get; init; }

    public int? Month { get; init; }

    public int? Day { get; init; }

    public SortDirection? SortByDate { get; init; }

    public SortDirection? SortBySum { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
