using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Events;

public sealed record PlayerUpdatedIntegrationEvent(LobbyPlayerModel LobbyPlayerModel, long LobbyId);