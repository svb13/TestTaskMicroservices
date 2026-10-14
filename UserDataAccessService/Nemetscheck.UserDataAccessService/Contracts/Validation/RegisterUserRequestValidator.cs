using FluentValidation;
using Nemetscheck.UserDataAccessService.Contracts.Requests;

namespace Nemetscheck.UserDataAccessService.Contracts.Validation;

internal sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(r => r.FirstName).PersonName("First name");
        RuleFor(r => r.LastName).PersonName("Last name");
        RuleFor(r => r.Email).Email();
        RuleFor(r => r.Password).Password();
    }
}
