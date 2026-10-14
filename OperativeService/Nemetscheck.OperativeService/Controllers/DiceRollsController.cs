using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nemetscheck.OperativeService.Authentication;
using Nemetscheck.OperativeService.Contracts.Requests;
using Nemetscheck.OperativeService.Contracts.Responses;
using Nemetscheck.OperativeService.Domain.DiceRolls;
using Nemetscheck.OperativeService.Services;

namespace Nemetscheck.OperativeService.Controllers;

[ApiController]
[Authorize]
[Route("api/dice-rolls")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
public sealed class DiceRollsController(IDiceRollService diceRollService, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<DiceRollResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> RollAsync(CancellationToken cancellationToken)
    {
        var roll = await diceRollService.RollAsync(currentUser.Id, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, roll);
    }

    [HttpGet]
    [ProducesResponseType<PagedResponse<DiceRollResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync([FromQuery] GetDiceRollsRequest request, CancellationToken cancellationToken)
    {
        var query = new DiceRollQuery(currentUser.Id, request.Year, request.Month, request.Day,
            request.SortByDate, request.SortBySum, request.Page, request.PageSize);

        return Ok(await diceRollService.GetAsync(query, cancellationToken));
    }
}
