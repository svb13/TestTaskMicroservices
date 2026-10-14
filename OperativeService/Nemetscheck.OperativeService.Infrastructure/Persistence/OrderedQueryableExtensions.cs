using System.Linq.Expressions;
using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Infrastructure.Persistence;

internal static class OrderedQueryableExtensions
{
    public static IOrderedQueryable<T> ThenBy<T, TKey>(this IOrderedQueryable<T> source, Expression<Func<T, TKey>> key, SortDirection direction) =>
        direction == SortDirection.Asc ? source.ThenBy(key) : source.ThenByDescending(key);
}
