using Games.Domain.Entities;
using Games.Domain.Enums;
using Games.Domain.Types;
using Shared.Domain.Enums;

namespace Games.Domain.Tests.Data;

public static class GameTestCases
{
    public static IEnumerable<Game> ValidGames()
    {
        yield return new()
        {
            LobbyId = 1,
            Players =
            [
                new Player
                {
                    UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                    Name = "Player1",
                    Colour = Colour.Red,
                    Points = 10,
                    TurnPosition = 0
                },
                new Player
                {
                    UserId = "1a6a2a19-d2a3-46eb-b732-da1550ccc8d1",
                    Name = "Player2",
                    Colour = Colour.Black,
                    Points = 0,
                    TurnPosition = 1
                }
            ],
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
        };

        // 3 players, the middle seat is in the action phase and two tiles of the first bucket are played
        yield return new()
        {
            LobbyId = 2,
            TurnPhase = TurnPhase.Actions,
            CurrentTurnPosition = 1,
            DrawnTiles = Drawn(TileCardIdentifier.A0, TileCardIdentifier.A3),
            Players =
            [
                new Player
                {
                    UserId = "5d0c1f5e-7c0b-4f0a-9a57-1c4c6b0f2f11",
                    Name = "Player1",
                    Colour = Colour.Green,
                    Points = 4,
                    TurnPosition = 0
                },
                new Player
                {
                    UserId = "b0a4c1de-3c52-4f7b-8a0e-0d7f3f6a9c22",
                    Name = "Player2",
                    Colour = Colour.Blue,
                    Points = 7,
                    TurnPosition = 1
                },
                new Player
                {
                    UserId = "c9e1b7a2-64d3-4b1f-b2c8-5a1e9d4f7b33",
                    Name = "Player3",
                    Colour = Colour.Yellow,
                    Points = 2,
                    TurnPosition = 2
                }
            ],
            Tiles =
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthEast: 1, South: 2),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new TreasureTile
                {
                    Costs = new TravelCosts(NorthWest: 1),
                    Coordinate = new HexCoordinate(1, 0)
                },
                new TempleTile
                {
                    TempleLevel = 2,
                    Costs = new TravelCosts(North: 1),
                    Coordinate = new HexCoordinate(0, 1)
                }
            ]
        };

        // 4 players, the last seat is about to place a tile, so the turn order wraps around next
        yield return new()
        {
            LobbyId = 3,
            TurnPhase = TurnPhase.PlaceTile,
            CurrentTurnPosition = 3,
            Players =
            [
                new Player
                {
                    UserId = "2f7d9a10-8b6e-4c3a-9d21-7e5b0c1a4d44",
                    Name = "Player1",
                    Colour = Colour.Red,
                    Points = 0,
                    TurnPosition = 0
                },
                new Player
                {
                    UserId = "8a3e6c54-1f2b-4d9e-a7c0-3b9d2e6f5a55",
                    Name = "Player2",
                    Colour = Colour.Black,
                    Points = 0,
                    TurnPosition = 1
                },
                new Player
                {
                    UserId = "e4b2d8f6-9a1c-4e7d-8f35-6c0a7b3d2e66",
                    Name = "Player3",
                    Colour = Colour.Green,
                    Points = 0,
                    TurnPosition = 2
                },
                new Player
                {
                    UserId = "71c5a9e3-0d4f-4b8a-b6e2-9f1d8c7a6b77",
                    Name = "Player4",
                    Colour = Colour.Blue,
                    Points = 0,
                    TurnPosition = 3
                }
            ],
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
        };

        // 2 players left with a gap in the turn order (seat 1 left the game), the highest seat is on turn
        yield return new()
        {
            LobbyId = 4,
            TurnPhase = TurnPhase.Actions,
            CurrentTurnPosition = 2,
            DrawnTiles = Drawn(TileCardIdentifier.A1, TileCardIdentifier.A2, TileCardIdentifier.A4),
            Players =
            [
                new Player
                {
                    UserId = "a6f0e2b8-5c7d-4a19-9e43-2d8b1f0c3a88",
                    Name = "Player1",
                    Colour = Colour.Yellow,
                    Points = 12,
                    TurnPosition = 0
                },
                new Player
                {
                    UserId = "d3b9c7a1-2e6f-4d58-8a04-4f7e6a5b9c99",
                    Name = "Player3",
                    Colour = Colour.Red,
                    Points = 9,
                    TurnPosition = 2
                }
            ],
            Tiles =
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(North: 1, SouthEast: 2),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new TempleTile
                {
                    TempleLevel = 1,
                    Costs = new TravelCosts(South: 1),
                    Coordinate = new HexCoordinate(0, -1)
                },
                new TempleTile
                {
                    TempleLevel = 2,
                    Costs = new TravelCosts(NorthWest: 2, NorthEast: 1),
                    Coordinate = new HexCoordinate(1, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthWest: 1),
                    Coordinate = new HexCoordinate(2, -1)
                }
            ]
        };
    }

    /// <summary>
    /// Builds a <see cref="Game.DrawnTiles"/> bit mask with the bits of the given tiles set.
    /// </summary>
    private static ulong Drawn(params TileCardIdentifier[] tiles)
    {
        return tiles.Aggregate(0UL, (mask, tile) => mask | 1UL << (int)tile);
    }
}