namespace Accounts.Contracts.Queries;

public sealed record GetAccountsQuery(ISet<string> UserIds);