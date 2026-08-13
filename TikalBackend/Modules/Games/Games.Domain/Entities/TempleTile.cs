using Games.Domain.Enums;

namespace Games.Domain.Entities;

public sealed class TempleTile : Tile
{
    public override TileType Type => TileType.Temple;
}