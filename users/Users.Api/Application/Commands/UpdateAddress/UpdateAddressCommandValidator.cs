using FluentValidation;

namespace Users.Api.Application.Commands.UpdateAddress;

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(c => c.Country)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(60);

        RuleFor(c => c.State)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100);

        RuleFor(c => c.City).NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100);

        RuleFor(c => c.ZipCode)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(10);

        RuleFor(c => c.Street)
            .MinimumLength(1)
            .MaximumLength(100);
    }
}