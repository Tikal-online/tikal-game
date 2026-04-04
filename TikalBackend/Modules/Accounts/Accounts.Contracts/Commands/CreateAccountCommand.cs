using Accounts.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Contracts.Messaging;

namespace Accounts.Contracts.Commands;

public sealed record CreateAccountCommand(string UserId, string Name) : Command<OneOf<Success, DuplicateUserId>>;