namespace Accounts.Application.DataAccess;

public interface UnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}