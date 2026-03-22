using Accounts.Contracts.Queries;
using FluentValidation;

namespace Accounts.Application.UseCases.GetAccount;

public sealed class GetAccountQueryValidator : AbstractValidator<GetAccountQuery>
{
    public GetAccountQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty")
            .MaximumLength(100)
            .WithMessage("UserId cannot exceed 100 characters");
    }
}