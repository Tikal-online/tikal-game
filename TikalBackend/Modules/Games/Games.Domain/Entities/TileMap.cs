using Games.Domain.Errors;
using Games.Domain.Types;
using OneOf;

namespace Games.Domain.Entities;

public sealed class TileMap
{
    public List<Tile> Tiles { get; init; } = [];

    public OneOf<int, NoPathFound> GetTravelCost(HexCoordinate start, HexCoordinate goal)
    {
        var dict = Tiles.ToDictionary(tile => tile.Coordinate);

        var frontier = new PriorityQueue<HexCoordinate, int>();
        frontier.Enqueue(start, 0);

        var cost_so_far = new Dictionary<HexCoordinate, int>
        {
            [start] = 0
        };

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
            {
                return cost_so_far[current];
            }

            foreach (var next in GetNeighbours(current, dict))
            {
                var travelCost = GetTileTravelCost(current, next, dict);

                if (travelCost == 0)
                {
                    continue;
                }

                var newCost = cost_so_far[current] + travelCost;

                if (!cost_so_far.TryGetValue(next, out var oldCost) || newCost < oldCost)
                {
                    cost_so_far[next] = newCost;
                    var priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                }
            }
        }

        return new NoPathFound(start, goal);
    }

    private static IEnumerable<HexCoordinate> GetNeighbours(
        HexCoordinate coordinate,
        Dictionary<HexCoordinate, Tile> dict
    )
    {
        foreach (var direction in HexCoordinate.Directions)
        {
            var neighbour = coordinate + direction;

            if (dict.ContainsKey(neighbour))
            {
                yield return neighbour;
            }
        }
    }

    private static int GetTileTravelCost(HexCoordinate a, HexCoordinate b, Dictionary<HexCoordinate, Tile> dict)
    {
        var tileAEdge = a.GetEdge(b);
        var tileBEdge = b.GetEdge(a);

        var tileA = dict[a];
        var tileB = dict[b];

        return tileA.Costs[tileAEdge] + tileB.Costs[tileBEdge];
    }

    private static int Heuristic(HexCoordinate a, HexCoordinate b)
    {
        return Math.Abs(a.Q - b.Q) + Math.Abs(a.R - b.R);
    }
}