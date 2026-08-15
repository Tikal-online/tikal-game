namespace Shared.Application.DataAccess;

public interface TransactionHandler
{
    Task<T> CommitScopedTransaction<T>(
        Func<CancellationToken, Task<T>> func,
        CancellationToken cancellationToken = default
    );
}