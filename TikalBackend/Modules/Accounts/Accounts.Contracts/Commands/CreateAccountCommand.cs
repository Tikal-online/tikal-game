using Accounts.Contracts.Errors;
using Accounts.Contracts.Models;
using OneOf;
using Shared.Contracts.Messaging;

namespace Accounts.Contracts.Commands;

public sealed record CreateAccountCommand(string UserId, string Name) : Command<OneOf<AccountModel, DuplicateUserId>>;