using Accounts.Contracts.Models;
using Shared.Contracts.Messaging;

namespace Accounts.Contracts.Queries;

public sealed record GetAccountsQuery(ISet<string> UserIds) : Query<List<AccountModel>>;