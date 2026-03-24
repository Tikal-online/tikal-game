using Accounts.Contracts.Models;
using Shared.Contracts.Messaging;

namespace Accounts.Contracts.Queries;

public sealed record GetAccountQuery(string UserId) : Query<AccountModel?>;