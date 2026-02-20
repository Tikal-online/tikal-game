using FluentValidation;
using Users.Contracts.Commands;

namespace Users.Application.Usecases.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .MinimumLength(6)
            .WithMessage("Username must be at least 6 characters long.")
            .MaximumLength(30)
            .WithMessage("Username must be at most 30 characters long.");

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(100)
            .WithMessage("Password must be at most 100 characters long.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special letter");
    }
}