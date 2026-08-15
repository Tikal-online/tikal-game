using System.Collections.Immutable;
using Games.Domain.Enums;

namespace Games.Domain.Types;

public readonly record struct HexCoordinate(int Q, int R)
{
    private static readonly ImmutableDictionary<HexCoordinate, Edge> Edges = new Dictionary<HexCoordinate, Edge>
    {
        [new HexCoordinate(0, -1)] = Edge.North,
        [new HexCoordinate(1, -1)] = Edge.NorthEast,
        [new HexCoordinate(1, 0)] = Edge.SouthEast,
        [new HexCoordinate(0, 1)] = Edge.South,
        [new HexCoordinate(-1, 1)] = Edge.SouthWest,
        [new HexCoordinate(-1, 0)] = Edge.NorthWest
    }.ToImmutableDictionary();

    public static readonly IReadOnlyList<HexCoordinate> Directions = [.. Edges.Keys];

    public static HexCoordinate operator +(HexCoordinate a, HexCoordinate b)
    {
        return new HexCoordinate(a.Q + b.Q, a.R + b.R);
    }

    public static HexCoordinate operator -(HexCoordinate a, HexCoordinate b)
    {
        return new HexCoordinate(a.Q - b.Q, a.R - b.R);
    }

    public Edge GetEdge(HexCoordinate neighbour)
    {
        var difference = neighbour - this;

        return Edges[difference];
    }
}