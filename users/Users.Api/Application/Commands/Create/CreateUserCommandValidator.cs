using FluentValidation;
using Users.Api.Application.Validation;

namespace Users.Api.Application.Commands.Create;

public partial class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50);

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
            .Must(ValidationHelper.BeValidBirthDate)
            .WithMessage("You must be at least 18 years old.");

        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(64);

        RuleFor(c => c.Address).NotNull();

        RuleFor(c => c.Address.Country)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(60)
            .When(c => c.Address is not null);

        RuleFor(c => c.Address.State)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100)
            .When(c => c.Address is not null);

        RuleFor(c => c.Address.City)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100)
            .When(c => c.Address is not null);

        RuleFor(c => c.Address.ZipCode)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(10)
            .When(c => c.Address is not null);

        RuleFor(c => c.Address.Street)
            .MinimumLength(1)
            .MaximumLength(100)
            .When(c => c.Address is not null);
    }
}