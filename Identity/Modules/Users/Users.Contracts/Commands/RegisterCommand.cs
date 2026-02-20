using OneOf;
using Shared.Contracts.Messaging;
using Users.Contracts.Dtos;
using Users.Contracts.Errors;

namespace Users.Contracts.Commands;

public sealed record RegisterCommand(string Username, string Password)
    : Command<OneOf<UserDto, DuplicateUsernameError>>;