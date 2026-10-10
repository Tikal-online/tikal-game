using Games.Domain.Entities;
using Games.Domain.Types;

namespace Games.Domain.Tests.Data;

// the test cases follow the structure: Tiles, start, goal, expected cost
internal class ValidTilePaths : TheoryData<List<Tile>, HexCoordinate, HexCoordinate, int>
{
    public ValidTilePaths()
    {
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
                    Coordinate = new HexCoordinate(1, -1)
                }
            ],
            new HexCoordinate(0, 0),
            new HexCoordinate(1, -1),
            2
        );
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthEast: 1),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthEast: 1, NorthWest: 1),
                    Coordinate = new HexCoordinate(1, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthWest: 1),
                    Coordinate = new HexCoordinate(2, 0)
                }
            ],
            new HexCoordinate(0, 0),
            new HexCoordinate(2, 0),
            4
        );
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthEast: 5, NorthEast: 1),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthWest: 5, SouthEast: 5, North: 5),
                    Coordinate = new HexCoordinate(1, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthEast: 5, North: 1),
                    Coordinate = new HexCoordinate(2, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthWest: 1, SouthEast: 1),
                    Coordinate = new HexCoordinate(1, -1)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthWest: 1, South: 1),
                    Coordinate = new HexCoordinate(2, -1)
                }
            ],
            new HexCoordinate(0, 0),
            new HexCoordinate(2, 0),
            6
        );
    }
}