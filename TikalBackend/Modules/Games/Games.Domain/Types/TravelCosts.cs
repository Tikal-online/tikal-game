using Games.Domain.Enums;

namespace Games.Domain.Types;

public readonly struct TravelCosts
{
    private readonly int[] costs;

    public TravelCosts(
        int north = 0,
        int northeast = 0,
        int southeast = 0,
        int south = 0,
        int southwest = 0,
        int northwest = 0
    )
    {
        costs = [north, northeast, southeast, south, southwest, northwest];
    }

    public int this[Edge edge] => costs[(int)edge];
}