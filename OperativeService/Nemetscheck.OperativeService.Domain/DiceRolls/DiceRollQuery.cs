namespace Nemetscheck.OperativeService.Domain.DiceRolls;

// null Year means all records, Month requires Year and Day requires Month
public sealed record DiceRollQuery(
    long UserId,
    int? Year,
    int? Month,
    int? Day,
    SortDirection? SortByDate,
    SortDirection? SortBySum,
    int Page,
    int PageSize);
