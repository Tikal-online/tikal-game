using Games.Domain.Enums;

namespace Games.Domain.Types;

public readonly record struct TravelCosts(
    int North = 0,
    int NorthEast = 0,
    int SouthEast = 0,
    int South = 0,
    int SouthWest = 0,
    int NorthWest = 0
)
{
    public int this[Edge edge]
    {
        get
        {
            return edge switch
            {
                Edge.North => North,
                Edge.NorthEast => NorthEast,
                Edge.SouthEast => SouthEast,
                Edge.South => South,
                Edge.SouthWest => SouthWest,
                Edge.NorthWest => NorthWest,
                _ => throw new ArgumentOutOfRangeException(nameof(edge), edge, null)
            };
        }
    }

    public TravelCosts Rotate(int steps)
    {
        int[] costs = [North, NorthEast, SouthEast, South, SouthWest, NorthWest];
        var rotated = new int[costs.Length];

        var shift = ((steps % costs.Length) + costs.Length) % costs.Length;

        for (var edge = 0; edge < costs.Length; edge++)
        {
            rotated[(edge + shift) % costs.Length] = costs[edge];
        }

        return new TravelCosts(rotated[0], rotated[1], rotated[2], rotated[3], rotated[4], rotated[5]);
    }
}