using Games.Domain.Enums;

namespace Games.Domain.Entities;

public sealed class TroopAssignment
{
    public long Id { get; set; }

    public long TileId { get; set; }

    public Tile Tile { get; set; } = null!;

    public required TroopType TroopType { get; set; }

    public required int Count { get; set; }

    public long PlayerId { get; set; }

    public Player Player { get; set; } = null!;
}