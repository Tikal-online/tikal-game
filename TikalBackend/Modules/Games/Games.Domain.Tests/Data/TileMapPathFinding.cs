using Games.Domain.Entities;
using Games.Domain.Types;

namespace Games.Domain.Tests.Data;

public static class TileMapPathFinding
{
    // the test cases follow the structure: Map, start, goal, expected cost
    public static IEnumerable<TestCaseData> ValidTestCases =>
    [
        // two adjacent tiles, each has a cost of 1 on the shared edge
        new(
            new TileMap
            {
                Tiles =
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
                ]
            },
            new HexCoordinate(0, 0),
            new HexCoordinate(1, -1),
            2
        ),
        // straight three-tile chain going southeast, forces routing through the middle tile
        new(
            new TileMap
            {
                Tiles =
                [
                    new EmptyTile
                    {
                        Costs = new TravelCosts(SouthEast: 1),
                        Coordinate = new HexCoordinate(0, 0)
                    },
                    new EmptyTile
                    {
                        Costs = new TravelCosts(SouthEast: 1, Northwest: 1),
                        Coordinate = new HexCoordinate(1, 0)
                    },
                    new EmptyTile
                    {
                        Costs = new TravelCosts(Northwest: 1),
                        Coordinate = new HexCoordinate(2, 0)
                    }
                ]
            },
            new HexCoordinate(0, 0),
            new HexCoordinate(2, 0),
            4
        ),
        // a cheap 3-hop route (cost 6) must beat more expensive 2-hop and 3 hop routes
        new(
            new TileMap
            {
                Tiles =
                [
                    new EmptyTile
                    {
                        Costs = new TravelCosts(SouthEast: 5, NorthEast: 1),
                        Coordinate = new HexCoordinate(0, 0)
                    },

                    new EmptyTile
                    {
                        Costs = new TravelCosts(Northwest: 5, SouthEast: 5, North: 5),
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
                        Costs = new TravelCosts(Northwest: 1, South: 1),
                        Coordinate = new HexCoordinate(2, -1)
                    }
                ]
            },
            new HexCoordinate(0, 0),
            new HexCoordinate(2, 0),
            6
        )
    ];

    // the test cases follow the structure: Map, start, goal
    public static IEnumerable<TestCaseData> NoPathTestCases =>
    [
        // two tiles without a connection of other tiles
        new(
            new TileMap
            {
                Tiles =
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
                ]
            },
            new HexCoordinate(0, 0),
            new HexCoordinate(2, -2)
        ),
        // two tiles with a cost of 0
        new(
            new TileMap
            {
                Tiles =
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
                ]
            },
            new HexCoordinate(0, 0),
            new HexCoordinate(1, -1)
        )
    ];
}