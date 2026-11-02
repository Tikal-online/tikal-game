using Games.Contracts.Models;

namespace Games.Contracts.Events;

public sealed record GameCompletedIntegrationEvent(GameModel Game);