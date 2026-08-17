using Accounts.Domain.Entities;

namespace Accounts.Application.DataAccess;

public interface AccountQueryContext
{
    Task<Account?> GetByUserId(string userId);

    Task<List<Account>> GetByUserIds(ISet<string> userIds);
}