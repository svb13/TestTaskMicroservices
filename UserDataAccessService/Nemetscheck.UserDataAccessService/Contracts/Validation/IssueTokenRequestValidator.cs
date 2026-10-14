using FluentValidation;
using Nemetscheck.UserDataAccessService.Contracts.Requests;

namespace Nemetscheck.UserDataAccessService.Contracts.Validation;

internal sealed class IssueTokenRequestValidator : AbstractValidator<IssueTokenRequest>
{
    public IssueTokenRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(r => r.Password).NotEmpty().MaximumLength(1024);
    }
}
