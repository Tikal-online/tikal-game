using Accounts.Domain.Entities;

namespace Accounts.Application.DataAccess;

public interface AccountQueryContext
{
    IQueryable<Account> Accounts { get; }
}