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
}