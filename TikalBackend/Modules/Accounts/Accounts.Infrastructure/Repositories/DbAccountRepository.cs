using Accounts.Application.DataAccess;
using Accounts.Domain.Entities;
using Accounts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Repositories;

internal sealed class DbAccountRepository : AccountRepository
{
    private readonly AccountsDbContext accountsDbContext;

    public DbAccountRepository(AccountsDbContext accountsDbContext)
    {
        this.accountsDbContext = accountsDbContext;
    }

    public void Create(Account account)
    {
        accountsDbContext.Add(account);
    }

    public Task<Account?> GetByUserIdAsync(string userId)
    {
        return accountsDbContext.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);
    }
}