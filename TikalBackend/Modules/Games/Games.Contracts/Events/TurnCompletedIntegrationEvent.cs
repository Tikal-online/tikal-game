using Games.Contracts.Models;

namespace Games.Contracts.Events;

public sealed record TurnCompletedIntegrationEvent(GamePlayerModel NewPlayer, long GameId);