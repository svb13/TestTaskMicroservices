using Microsoft.EntityFrameworkCore;
using Nemetscheck.OperativeService.Domain;
using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Infrastructure.Persistence;

internal sealed class DiceRollRepository(OperativeDbContext dbContext) : IDiceRollRepository
{
    public async Task AddAsync(DiceRoll roll, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roll);
        
        await dbContext.DiceRolls.AddAsync(roll, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedResult<DiceRoll>> QueryAsync(DiceRollQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var rolls = dbContext.DiceRolls.AsNoTracking().Where(r => r.UserId == query.UserId);

        if (GetPeriod(query) is var (from, to))
            rolls = rolls.Where(r => r.RolledAtUtc >= from && r.RolledAtUtc < to);

        var totalCount = await rolls.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await ApplySorting(rolls, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<DiceRoll>(items, query.Page, query.PageSize, totalCount);
    }

    private static (DateTime From, DateTime To)? GetPeriod(DiceRollQuery query)
    {
        if (query.Year is not { } year)
            return null;

        if (query.Month is not { } month)
        {
            var yearStart = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (yearStart, EndOrMax(yearStart, static d => d.AddYears(1)));
        }

        if (query.Day is not { } day)
        {
            var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            return (monthStart, EndOrMax(monthStart, static d => d.AddMonths(1)));
        }

        var dayStart = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);

        return (dayStart, EndOrMax(dayStart, static d => d.AddDays(1)));
    }

    // the period ending after year 9999 cannot be represented, so it is capped at DateTime.MaxValue
    private static DateTime EndOrMax(DateTime start, Func<DateTime, DateTime> next)
    {
        try
        {
            return next(start);
        }
        catch (ArgumentOutOfRangeException)
        {
            return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
        }
    }

    private static IQueryable<DiceRoll> ApplySorting(IQueryable<DiceRoll> rolls, DiceRollQuery query)
    {
        IOrderedQueryable<DiceRoll> ordered = (query.SortBySum, query.SortByDate) switch
        {
            ({ } sum, { } date) => OrderBy(rolls, r => r.Sum, sum).ThenBy(r => r.RolledAtUtc, date),
            ({ } sum, null) => OrderBy(rolls, r => r.Sum, sum),
            (null, { } date) => OrderBy(rolls, r => r.RolledAtUtc, date),
            _ => rolls.OrderByDescending(r => r.RolledAtUtc)
        };

        var tieBreakerDirection = query.SortByDate ?? (query.SortBySum is null ? SortDirection.Desc : SortDirection.Asc);
        
        return ordered.ThenBy(r => r.Id, tieBreakerDirection);
    }

    private static IOrderedQueryable<DiceRoll> OrderBy<TKey>(IQueryable<DiceRoll> source, System.Linq.Expressions.Expression<Func<DiceRoll, TKey>> key, SortDirection direction) =>
        direction == SortDirection.Asc ? source.OrderBy(key) : source.OrderByDescending(key);
}
