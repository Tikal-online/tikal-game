using Accounts.Contracts.Queries;
using Accounts.Domain.Entities;
using FluentValidation;

namespace Accounts.Application.UseCases.GetAccount;

public sealed class GetAccountQueryValidator : AbstractValidator<GetAccountQuery>
{
    public GetAccountQueryValidator()
    {
        RuleFor(x => x.UserId)
            .ValidAccountUserId();
    }
}