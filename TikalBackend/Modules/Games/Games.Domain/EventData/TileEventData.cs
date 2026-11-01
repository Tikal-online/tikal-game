using Games.Domain.Enums;
using Games.Domain.Types;

namespace Games.Domain.EventData;

public abstract record TileEventData
{
    public required TileType Type { get; set; }

    public required TravelCosts Costs { get; set; }

    public required HexCoordinate Coordinate { get; set; }
}