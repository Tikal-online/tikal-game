using Games.Domain.Types;

namespace Games.Domain.Entities;

/// <summary>
/// Uniquely identifies every tile card in the draw stack.
/// Naming follows the different draw buckets (A - G).
/// 36 in total since there are 36 cards in the draw stack.
/// </summary>
public enum TileCardIdentifier : byte
{
    A0 = 0, A1, A2, A3, A4, A5,
    B0, B1, B2, B3, B4, B5,
    C0, C1, C2, C3, C4,
    D0, D1, D2, D3, D4,
    E0, E1, E2, E3, E4,
    F0, F1, F2, F3, F4,
    G0, G1, G2, G3
}

public static class TileCatalogue
{
    /// <summary>
    /// Stores which indexes are in which buckets.
    /// Buckets are sorted: first entry is the first bucket.
    /// One entry is one bucket and the valid indexes are marked via the set bits of the ulong.
    /// E.g:
    /// ...0000111111000000 means that card indexes 6 - 11 are part of this bucket.
    /// The index corresponds to the numberic value of TileCardIdentifier which can then be
    /// used to look up the Tile.
    /// </summary>
    public static ulong[] BucketBitMasks { get; } = [
        Mask(0, 6),
        Mask(6, 6),
        Mask(12, 5),
        Mask(17, 5),
        Mask(22, 5),
        Mask(27, 5),
        Mask(32, 4),
    ];

    /// <summary>
    /// Creates a ulong with <paramref name="count"/> consecutive bits set,
    /// starting at bit index <paramref name="first"/>.
    /// E.g: first = 2, count = 4
    /// ...000000111100
    /// </summary>
    private static ulong Mask(int first, int count)
    {
        return ((1UL << count) - 1) << first;
    }

    public static Tile CreateTile(TileCardIdentifier tileCardIdentifier, HexCoordinate coordinate = default)
    {
        return TileFactories[tileCardIdentifier](coordinate);
    }

    // TODO: add the rest of the tiles
    private static readonly Dictionary<TileCardIdentifier, Func<HexCoordinate, Tile>> TileFactories = new()
    {
        [TileCardIdentifier.A0] = coordinate => new TreasureTile
        {
            Costs = new TravelCosts(North: 2, SouthWest: 1),
            Coordinate = coordinate
        },
        [TileCardIdentifier.A1] = coordinate => new TempleTile
        {
            TempleLevel = 1,
            Costs = new TravelCosts(South: 2, SouthWest: 1),
            Coordinate = coordinate
        },
        [TileCardIdentifier.A2] = coordinate => new TempleTile
        {
            TempleLevel = 2,
            Costs = new TravelCosts(North: 1, South: 2),
            Coordinate = coordinate
        },
        [TileCardIdentifier.A3] = coordinate => new TempleTile
        {
            TempleLevel = 2,
            Costs = new TravelCosts(South: 2, NorthWest: 1),
            Coordinate = coordinate
        },
        [TileCardIdentifier.A4] = coordinate => new EmptyTile
        {
            Costs = new TravelCosts(North: 1, South: 1, SouthWest: 1),
            Coordinate = coordinate
        },
        [TileCardIdentifier.A5] = coordinate => new TreasureTile
        {
            Costs = new TravelCosts(South: 1, NorthEast: 2),
            Coordinate = coordinate
        },
    };

}