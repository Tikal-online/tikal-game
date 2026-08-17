using Accounts.Application.DataAccess;
using Accounts.Domain.Entities;
using Accounts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.QueryContexts;

internal sealed class DbAccountQueryContext : AccountQueryContext
{
    private readonly AccountsDbContext accountsDbContext;

    public DbAccountQueryContext(AccountsDbContext accountsDbContext)
    {
        this.accountsDbContext = accountsDbContext;
    }

    public Task<Account?> GetByUserId(string userId)
    {
        return accountsDbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public Task<List<Account>> GetByUserIds(ISet<string> userIds)
    {
        return accountsDbContext.Accounts.AsNoTracking().Where(x => userIds.Contains(x.UserId)).ToListAsync();
    }
}