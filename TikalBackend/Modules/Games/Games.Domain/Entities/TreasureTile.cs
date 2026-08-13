using Games.Domain.Enums;

namespace Games.Domain.Entities;

public sealed class TreasureTile : Tile
{
    public override TileType Type => TileType.Treasure;
}