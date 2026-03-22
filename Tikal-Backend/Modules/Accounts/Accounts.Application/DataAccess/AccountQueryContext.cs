using Accounts.Domain.Entities;

namespace Accounts.Application.DataAccess;

public interface AccountQueryContext
{
    Task<Account?> GetByUserIdAsync(string userId);
}