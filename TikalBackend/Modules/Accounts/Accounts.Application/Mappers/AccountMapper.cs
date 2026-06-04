using Accounts.Contracts.Models;
using Accounts.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Accounts.Application.Mappers;

[Mapper]
internal static partial class AccountMapper
{
    public static partial List<AccountModel> AccountsToAccountModels(IEnumerable<Account> account);
}