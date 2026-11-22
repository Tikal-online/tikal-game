using Games.Contracts.Models;

namespace Games.Contracts.Events;

public sealed record TilePlacedIntegrationEvent(TileModel Tile, long GameId);