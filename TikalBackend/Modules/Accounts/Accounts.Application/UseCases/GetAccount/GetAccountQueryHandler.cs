using Accounts.Application.DataAccess;
using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;

namespace Accounts.Application.UseCases.GetAccount;

public sealed class GetAccountQueryHandler
{
    private readonly AccountQueryContext accountQueryContext;

    public GetAccountQueryHandler(AccountQueryContext accountQueryContext)
    {
        this.accountQueryContext = accountQueryContext;
    }

    public async Task<AccountModel?> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await accountQueryContext.GetByUserId(request.UserId);

        return account is null ? null : new AccountModel { UserId = account.UserId, Name = account.Name };
    }
}