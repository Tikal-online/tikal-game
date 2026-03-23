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

    public Task<Account?> GetByUserIdAsync(string userId)
    {
        return accountsDbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
    }
}