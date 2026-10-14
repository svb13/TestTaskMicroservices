using FluentValidation;
using Nemetscheck.OperativeService.Contracts.Requests;

namespace Nemetscheck.OperativeService.Contracts.Validation;

internal sealed class GetDiceRollsRequestValidator : AbstractValidator<GetDiceRollsRequest>
{
    public const int MaxPageSize = 100;

    public GetDiceRollsRequestValidator()
    {
        RuleFor(r => r.Year).InclusiveBetween(1, 9999);
        RuleFor(r => r.Month).InclusiveBetween(1, 12);
        RuleFor(r => r.Day).InclusiveBetween(1, 31);

        RuleFor(r => r.Year).NotNull().When(r => r.Month is not null).WithMessage("Year is required when Month is specified");
        RuleFor(r => r.Month).NotNull().When(r => r.Day is not null).WithMessage("Month is required when Day is specified");

        RuleFor(r => r.Day)
            .Must((r, day) => day <= DateTime.DaysInMonth(r.Year!.Value, r.Month!.Value))
            .When(r => r.Year is >= 1 and <= 9999 && r.Month is >= 1 and <= 12 && r.Day is >= 1 and <= 31)
            .WithMessage("Day does not exist in the specified month");

        RuleFor(r => r.SortByDate).IsInEnum();
        RuleFor(r => r.SortBySum).IsInEnum();

        RuleFor(r => r.Page).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).InclusiveBetween(1, MaxPageSize);
    }
}
