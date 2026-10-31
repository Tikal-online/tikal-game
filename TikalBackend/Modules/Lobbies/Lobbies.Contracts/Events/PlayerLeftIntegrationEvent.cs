using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Events;

public sealed record PlayerLeftIntegrationEvent(LobbyPlayerModel LobbyPlayerModel, long LobbyId);