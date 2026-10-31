using Accounts.Application.DataAccess;
using Accounts.Application.Mappers;
using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;

namespace Accounts.Application.UseCases.GetAccounts;

public sealed class GetAccountsQueryHandler
{
    private readonly AccountQueryContext accountQueryContext;

    public GetAccountsQueryHandler(AccountQueryContext accountQueryContext)
    {
        this.accountQueryContext = accountQueryContext;
    }

    public async Task<List<AccountModel>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountQueryContext.GetByUserIds(request.UserIds);

        var accountModels = AccountMapper.AccountsToAccountModels(accounts);

        return accountModels;
    }
}