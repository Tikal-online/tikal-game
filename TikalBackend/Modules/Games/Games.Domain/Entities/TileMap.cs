using System.Collections;
using Games.Domain.Errors;
using Games.Domain.Types;
using OneOf;

namespace Games.Domain.Entities;

public sealed class TileMap : IEnumerable<KeyValuePair<HexCoordinate, Tile>>
{
    private readonly Dictionary<HexCoordinate, Tile> tiles = [];

    public List<KeyValuePair<HexCoordinate, Tile>> Tiles => tiles.ToList();

    public IEnumerator<KeyValuePair<HexCoordinate, Tile>> GetEnumerator()
    {
        return tiles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(HexCoordinate coordinate, Tile tile)
    {
        tiles.Add(coordinate, tile);
    }

    public Tile? GetTile(HexCoordinate coordinate)
    {
        return tiles.GetValueOrDefault(coordinate);
    }

    public bool SetTile(HexCoordinate coordinate, Tile newTile)
    {
        return tiles.TryAdd(coordinate, newTile);
    }

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

            foreach (var next in GetNeighbours(current))
            {
                var travelCost = GetTileTravelCost(current, next);

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

    private IEnumerable<HexCoordinate> GetNeighbours(HexCoordinate coordinate)
    {
        foreach (var direction in HexCoordinate.Directions.Keys)
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