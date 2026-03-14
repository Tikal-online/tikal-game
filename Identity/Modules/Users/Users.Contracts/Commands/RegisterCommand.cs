using OneOf;
using OneOf.Types;
using Shared.Contracts.Messaging;
using Users.Contracts.Errors;

namespace Users.Contracts.Commands;

public sealed record RegisterCommand(string Username, string Password)
    : Command<OneOf<Success, DuplicateUsernameError>>;