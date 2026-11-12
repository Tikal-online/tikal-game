using Games.Contracts.Models;

namespace Games.Contracts.Events;

public sealed record PlayerLeftIntegrationEvent(GamePlayerModel GamePlayerModel, long LobbyId);