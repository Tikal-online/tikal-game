using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Models;
using OneOf;
using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Commands;

public sealed record CreateLobbyCommand(string UserId, string Name, int MaxPlayers)
    : Command<OneOf<LobbyModel, MissingUserAccount, PlayerAlreadyInALobby>>;