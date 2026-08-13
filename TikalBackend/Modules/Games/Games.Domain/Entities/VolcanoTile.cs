using Games.Domain.Enums;

namespace Games.Domain.Entities;

public sealed class VolcanoTile : Tile
{
    public override TileType Type => TileType.Volcano;
}