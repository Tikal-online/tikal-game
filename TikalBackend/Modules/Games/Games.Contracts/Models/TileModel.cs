using Games.Contracts.Enums;

namespace Games.Contracts.Models;

public abstract record TileModel
{
    public required TileTypeModel Type { get; set; }

    public required TravelCostModel Costs { get; set; }

    public required HexCoordinateModel Coordinate { get; set; }
}