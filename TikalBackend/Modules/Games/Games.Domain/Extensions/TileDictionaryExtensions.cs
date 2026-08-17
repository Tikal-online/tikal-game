using Games.Domain.Entities;
using Games.Domain.Errors;
using Games.Domain.Types;
using OneOf;

namespace Games.Domain.Extensions;

public static class TileDictionaryExtensions
{
    extension(Dictionary<HexCoordinate, Tile> tiles)
    {
        public OneOf<int, NoPathFound> GetTravelCost(HexCoordinate start, HexCoordinate goal)
        {
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

                foreach (var next in tiles.GetNeighbours(current))
                {
                    var travelCost = tiles.GetTileTravelCost(current, next);

                    if (travelCost == 0)
                    {
                        continue;
                    }

                    var newCost = cost_so_far[current] + travelCost;

                    if (!cost_so_far.TryGetValue(next, out var oldCost) || newCost < oldCost)
                    {
                        cost_so_far[next] = newCost;
                        var priority = newCost + Dictionary<HexCoordinate, Tile>.Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                    }
                }
            }

            return new NoPathFound(start, goal);
        }

        private IEnumerable<HexCoordinate> GetNeighbours(HexCoordinate coordinate)
        {
            foreach (var direction in HexCoordinate.Directions)
            {
                var neighbour = coordinate + direction;

                if (tiles.ContainsKey(neighbour))
                {
                    yield return neighbour;
                }
            }
        }

        private int GetTileTravelCost(HexCoordinate a, HexCoordinate b)
        {
            var tileAEdge = a.GetEdge(b);
            var tileBEdge = b.GetEdge(a);

            var tileA = tiles[a];
            var tileB = tiles[b];

            return tileA.Costs[tileAEdge] + tileB.Costs[tileBEdge];
        }

        private static int Heuristic(HexCoordinate a, HexCoordinate b)
        {
            return Math.Abs(a.Q - b.Q) + Math.Abs(a.R - b.R);
        }
    }
}