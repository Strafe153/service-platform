using FluentValidation;
using Users.Application.Helpers;

namespace Users.Application.Commands.Update.Current;

public class UpdateCurrentUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateCurrentUserCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(15)
            .Matches(ValidationHelper.PhoneNumber());

        RuleFor(c => c.BirthDate)
            .NotEmpty()
            .Must(ValidationHelper.BeValidBirthDate);
    }
}