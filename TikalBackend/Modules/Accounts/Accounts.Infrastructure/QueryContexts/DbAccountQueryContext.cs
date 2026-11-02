using Accounts.Application.DataAccess;
using Accounts.Domain.Entities;
using Accounts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.QueryContexts;

public sealed class DbAccountQueryContext : AccountQueryContext
{
    private readonly AccountsDbContext accountsDbContext;

    public DbAccountQueryContext(AccountsDbContext accountsDbContext)
    {
        this.accountsDbContext = accountsDbContext;
    }

    public Task<Account?> GetByUserId(string userId, CancellationToken cancellationToken)
    {
        return accountsDbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public Task<List<Account>> GetByUserIds(ISet<string> userIds, CancellationToken cancellationToken)
    {
        return accountsDbContext.Accounts.AsNoTracking().Where(x => userIds.Contains(x.UserId)).ToListAsync(cancellationToken);
    }
}