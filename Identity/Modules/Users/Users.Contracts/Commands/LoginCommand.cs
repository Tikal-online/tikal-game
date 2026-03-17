using OneOf;
using Shared.Contracts.Messaging;
using Users.Contracts.Errors;
using Users.Contracts.Models;

namespace Users.Contracts.Commands;

public sealed record LoginCommand(string Username, string Password)
    : Command<OneOf<TokenPair, InvalidCredentialsError>>;