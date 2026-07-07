using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Commands;

public sealed record JoinLobbyCommand(long LobbyId)
    : Command<OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull>>;