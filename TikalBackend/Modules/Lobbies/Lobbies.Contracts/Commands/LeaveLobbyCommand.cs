using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Commands;

public sealed record LeaveLobbyCommand(long Id) : Command<OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby>>;