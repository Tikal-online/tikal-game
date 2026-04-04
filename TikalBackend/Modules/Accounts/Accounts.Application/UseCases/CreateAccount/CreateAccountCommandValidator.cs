using Accounts.Contracts.Commands;
using Accounts.Domain.Entities;
using FluentValidation;

namespace Accounts.Application.UseCases.CreateAccount;

public sealed class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.UserId)
            .ValidAccountUserId();

        RuleFor(x => x.Name)
            .ValidAccountName();
    }
}