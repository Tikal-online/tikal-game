using Games.Domain.Enums;

namespace Games.Domain.Entities;

public sealed class EmptyTile : Tile
{
    public override TileType Type => TileType.Empty;
}