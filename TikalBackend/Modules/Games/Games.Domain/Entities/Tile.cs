using Games.Domain.Enums;
using Games.Domain.Types;

namespace Games.Domain.Entities;

public abstract class Tile
{
    public abstract TileType Type { get; }

    public required TravelCosts Costs { get; init; }

    public required HexCoordinate Coordinate { get; init; }
}