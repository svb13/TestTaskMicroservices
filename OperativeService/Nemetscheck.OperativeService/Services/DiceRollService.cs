using Nemetscheck.OperativeService.Contracts.Responses;
using Nemetscheck.OperativeService.Domain;
using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Services;

internal sealed class DiceRollService(
    IDiceRollRepository rolls,
    IUnitOfWork unitOfWork,
    IDiceRoller diceRoller,
    TimeProvider timeProvider) : IDiceRollService
{
    public async Task<DiceRollResponse> RollAsync(long userId, CancellationToken cancellationToken)
    {
        var roll = DiceRoll.Create(userId, diceRoller.Roll(), diceRoller.Roll(), timeProvider.GetUtcNow().UtcDateTime);

        await rolls.AddAsync(roll, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(roll);
    }

    public async Task<PagedResponse<DiceRollResponse>> GetAsync(DiceRollQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = await rolls.QueryAsync(query, cancellationToken);

        return new PagedResponse<DiceRollResponse>(
            [.. page.Items.Select(ToResponse)], page.Page, page.PageSize, page.TotalCount, page.TotalPages);
    }

    private static DiceRollResponse ToResponse(DiceRoll roll) =>
        new(roll.Id, roll.FirstDie, roll.SecondDie, roll.Sum, DateTime.SpecifyKind(roll.RolledAtUtc, DateTimeKind.Utc));
}
