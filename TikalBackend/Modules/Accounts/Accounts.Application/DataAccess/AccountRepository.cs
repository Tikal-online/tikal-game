using Accounts.Domain.Entities;

namespace Accounts.Application.DataAccess;

public interface AccountRepository
{
    void Create(Account account);

    Task<Account?> GetByUserIdAsync(string userId);
}