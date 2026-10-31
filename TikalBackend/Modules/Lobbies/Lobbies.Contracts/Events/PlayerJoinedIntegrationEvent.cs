using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Events;

public sealed record PlayerJoinedIntegrationEvent(LobbyPlayerModel LobbyPlayerModel, long LobbyId);