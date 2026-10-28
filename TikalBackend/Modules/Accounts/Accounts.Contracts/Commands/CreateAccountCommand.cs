namespace Accounts.Contracts.Commands;

public sealed record CreateAccountCommand(string UserId, string Name);