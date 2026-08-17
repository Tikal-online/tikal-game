using Games.Domain.Entities;
using Games.Domain.Types;
using Shared.Domain.Enums;

namespace Games.Domain.Tests.Data;

public static class GameTestCases
{
    // TODO: add more game test cases
    public static IEnumerable<Game> ValidGameTestCases =>
    [
        new()
        {
            LobbyId = 1,
            Players =
            [
                new Player
                {
                    UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                    Colour = Colour.Red,
                    Points = 10
                },
                new Player
                {
                    UserId = "1a6a2a19-d2a3-46eb-b732-da1550ccc8d1",
                    Colour = Colour.Black,
                    Points = 0
                }
            ],
            TileMap = new TileMap
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
                    },
                    new VolcanoTile
                    {
                        Costs = new TravelCosts(),
                        Coordinate = new HexCoordinate(2, -1)
                    }
                ]
            }
        }
    ];
}