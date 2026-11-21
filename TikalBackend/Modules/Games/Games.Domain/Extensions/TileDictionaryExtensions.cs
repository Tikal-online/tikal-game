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

        public bool CanBePlaced(Tile tile)
        {
            if (tiles.ContainsKey(tile.Coordinate))
            {
                return false;
            }

            foreach (var neighbour in tiles.GetNeighbours(tile.Coordinate))
            {
                var travelCost = tiles.GetTileTravelCost(neighbour, tile);

                if (travelCost > 0)
                {
                    return true;
                }
            }

            return false;
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

        private int GetTileTravelCost(HexCoordinate coordinate, Tile tile)
        {
            var existingEdge = coordinate.GetEdge(tile.Coordinate);
            var providedEdge = tile.Coordinate.GetEdge(coordinate);

            var existingTile = tiles[coordinate];

            return existingTile.Costs[existingEdge] + tile.Costs[providedEdge];
        }

        private static int Heuristic(HexCoordinate a, HexCoordinate b)
        {
            var dq = a.Q - b.Q;
            var dr = a.R - b.R;

            return (Math.Abs(dq) + Math.Abs(dr) + Math.Abs(dq + dr)) / 2;
        }
    }
}