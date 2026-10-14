using FluentValidation;
using Nemetscheck.UserDataAccessService.Domain.Users;

namespace Nemetscheck.UserDataAccessService.Contracts.Validation;

internal static class DomainRuleExtensions
{
    public static IRuleBuilderOptionsConditions<T, string> PersonName<T>(this IRuleBuilder<T, string> ruleBuilder, string field) =>
        ruleBuilder.Custom((value, context) => AddFailure(context, PersonNameRules.Validate(value, field)));

    public static IRuleBuilderOptionsConditions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Custom((value, context) => AddFailure(context, EmailRules.Validate(value)));

    public static IRuleBuilderOptionsConditions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Custom((value, context) => AddFailure(context, PasswordPolicy.Validate(value)));

    private static void AddFailure<T>(ValidationContext<T> context, string? error)
    {
        if (error is not null)
            context.AddFailure(error);
    }
}
