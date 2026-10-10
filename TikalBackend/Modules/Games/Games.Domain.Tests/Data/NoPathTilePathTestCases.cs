using Games.Domain.Entities;
using Games.Domain.Types;

namespace Games.Domain.Tests.Data;

// the test cases follow the structure: Map, start, goal
internal class NoPathTilePaths : TheoryData<List<Tile>, HexCoordinate, HexCoordinate>
{
    public NoPathTilePaths()
    {
        // two tiles without a connection of other tiles
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthEast: 1),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthWest: 1),
                    Coordinate = new HexCoordinate(2, -2)
                }
            ],
            new HexCoordinate(0, 0),
            new HexCoordinate(2, -2)
        );
        // two tiles with a cost of 0
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthEast: 0),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthWest: 0),
                    Coordinate = new HexCoordinate(1, -1)
                }
            ],
            new HexCoordinate(0, 0),
            new HexCoordinate(1, -1)
        );
    }
}