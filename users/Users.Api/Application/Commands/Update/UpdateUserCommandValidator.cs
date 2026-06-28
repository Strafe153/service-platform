using FluentValidation;
using MediatR;
using Users.Api.Application.Validation;

namespace Users.Api.Application.Commands.Update;

public class UpdateUserCommandValidator
    : AbstractValidator<IdentifiedCommand<Guid, UpdateUserCommand, Unit>>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.Command.FirstName)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(c => c.Command.LastName)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(c => c.Command.PhoneNumber)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(15)
            .Matches(ValidationHelper.PhoneNumber());

        RuleFor(c => c.Command.BirthDate)
            .NotEmpty()
            .Must(ValidationHelper.BeValidBirthDate);
    }
}