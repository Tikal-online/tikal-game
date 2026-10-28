using Accounts.Application.DataAccess;
using Accounts.Contracts.Commands;
using Accounts.Contracts.Errors;
using Accounts.Contracts.Models;
using Accounts.Domain.Entities;
using OneOf;

namespace Accounts.Application.UseCases.CreateAccount;

public sealed class CreateAccountCommandHandler
{
    private readonly AccountRepository accountRepository;

    public CreateAccountCommandHandler(AccountRepository accountRepository)
    {
        this.accountRepository = accountRepository;
    }

    public async Task<OneOf<AccountModel, DuplicateUserId>> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingAccount = await accountRepository.GetByUserId(request.UserId);

        if (existingAccount is not null)
        {
            return new DuplicateUserId(request.UserId);
        }

        var newAccount = new Account
        {
            UserId = request.UserId,
            Name = request.Name
        };

        accountRepository.Create(newAccount);

        return new AccountModel { UserId = newAccount.UserId, Name = newAccount.Name };
    }
}