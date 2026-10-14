using System.Security.Cryptography;
using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Infrastructure.Dice;

internal sealed class RandomDiceRoller : IDiceRoller
{
    public int Roll() => RandomNumberGenerator.GetInt32(DiceRoll.MinFace, DiceRoll.MaxFace + 1);
}
