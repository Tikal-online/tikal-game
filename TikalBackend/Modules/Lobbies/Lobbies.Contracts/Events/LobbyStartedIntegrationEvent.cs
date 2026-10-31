using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Events;

public sealed record LobbyStartedIntegrationEvent(LobbyModel Lobby);