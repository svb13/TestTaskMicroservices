using Nemetscheck.OperativeService.Contracts.Responses;
using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Services;

public interface IDiceRollService
{
    Task<DiceRollResponse> RollAsync(long userId, CancellationToken cancellationToken);

    Task<PagedResponse<DiceRollResponse>> GetAsync(DiceRollQuery query, CancellationToken cancellationToken);
}
