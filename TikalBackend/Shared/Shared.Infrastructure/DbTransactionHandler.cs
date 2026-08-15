using System.Transactions;
using OneOf.Types;
using Shared.Application.DataAccess;

namespace Shared.Infrastructure;

internal sealed class DbTransactionHandler : TransactionHandler
{
    public async Task<T> CommitScopedTransaction<T>(
        Func<CancellationToken, Task<T>> func,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled
        );

        var result = await func(cancellationToken);

        if (result is not Success)
        {
            return result;
        }

        transaction.Complete();

        return result;
    }
}