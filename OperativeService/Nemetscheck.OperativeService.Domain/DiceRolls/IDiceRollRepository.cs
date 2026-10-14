namespace Nemetscheck.OperativeService.Domain.DiceRolls;

public interface IDiceRollRepository
{
    Task AddAsync(DiceRoll roll, CancellationToken cancellationToken);

    Task<PagedResult<DiceRoll>> QueryAsync(DiceRollQuery query, CancellationToken cancellationToken);
}
