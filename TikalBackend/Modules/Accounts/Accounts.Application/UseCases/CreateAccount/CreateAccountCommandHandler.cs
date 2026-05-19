using Accounts.Application.DataAccess;
using Accounts.Contracts.Commands;
using Accounts.Contracts.Errors;
using Accounts.Contracts.Models;
using Accounts.Domain.Entities;
using OneOf;
using Shared.Contracts.Messaging;

namespace Accounts.Application.UseCases.CreateAccount;

internal sealed class CreateAccountCommandHandler
    : CommandHandler<CreateAccountCommand, OneOf<AccountModel, DuplicateUserId>>
{
    private readonly AccountRepository accountRepository;

    private readonly UnitOfWork unitOfWork;

    public CreateAccountCommandHandler(AccountRepository accountRepository, UnitOfWork unitOfWork)
    {
        this.accountRepository = accountRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<OneOf<AccountModel, DuplicateUserId>> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingAccount = await accountRepository.GetByUserIdAsync(request.UserId);

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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccountModel { UserId = newAccount.UserId, Name = newAccount.Name };
    }
}