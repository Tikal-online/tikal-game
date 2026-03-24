using Accounts.Application.DataAccess;
using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Shared.Contracts.Messaging;

namespace Accounts.Application.UseCases.GetAccount;

internal sealed class GetAccountQueryHandler : QueryHandler<GetAccountQuery, AccountModel?>
{
    private readonly AccountQueryContext accountQueryContext;

    public GetAccountQueryHandler(AccountQueryContext accountQueryContext)
    {
        this.accountQueryContext = accountQueryContext;
    }

    public async Task<AccountModel?> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await accountQueryContext.GetByUserIdAsync(request.UserId);

        return account is null ? null : new AccountModel { UserId = account.UserId, Name = account.Name };
    }
}