using Accounts.Application.DataAccess;
using Accounts.Application.Mappers;
using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Shared.Contracts.Messaging;

namespace Accounts.Application.UseCases.GetAccounts;

internal sealed class GetAccountsQueryHandler : QueryHandler<GetAccountsQuery, List<AccountModel>>
{
    private readonly AccountQueryContext accountQueryContext;

    public GetAccountsQueryHandler(AccountQueryContext accountQueryContext)
    {
        this.accountQueryContext = accountQueryContext;
    }

    public async Task<List<AccountModel>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountQueryContext.GetByUserIdsAsync(request.UserIds);

        var accountModels = AccountMapper.AccountsToAccountModels(accounts);

        return accountModels;
    }
}