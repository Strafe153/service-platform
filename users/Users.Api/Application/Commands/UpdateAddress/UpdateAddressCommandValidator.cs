using FluentValidation;
using MediatR;

namespace Users.Api.Application.Commands.UpdateAddress;

public class UpdateAddressCommandValidator
    : AbstractValidator<IdentifiedCommand<Guid, UpdateAddressCommand, Unit>>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(c => c.Command.Country)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(60);

        RuleFor(c => c.Command.State)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100);

        RuleFor(c => c.Command.City).NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100);

        RuleFor(c => c.Command.ZipCode)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(10);

        RuleFor(c => c.Command.Street)
            .MinimumLength(1)
            .MaximumLength(100);
    }
}