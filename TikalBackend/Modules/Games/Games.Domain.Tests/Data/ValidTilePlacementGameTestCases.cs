using Games.Domain.Entities;
using Games.Domain.Enums;
using Games.Domain.Types;
using Shared.Domain.Enums;

namespace Games.Domain.Tests.Data;

// the test cases follow the structure: game with a drawn tile, location, rotation
internal class ValidTilePlacementGames : TheoryData<Game, HexCoordinate, int>
{
    public ValidTilePlacementGames()
    {
        // A1 (south 2): its south edge faces (1, -1)
        Add(
            new Game
            {
                LobbyId = 1,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A1,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A1,
                Players =
                [
                    new Player
                    {
                        UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                        Name = "Player1",
                        Colour = Colour.Red,
                        Points = 0,
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
                    }
                ]
            },
            new HexCoordinate(1, -2),
            0
        );
        // A0 (north 2) turned half way: north becomes south and faces (1, -1)
        Add(
            new Game
            {
                LobbyId = 2,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A0,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A0,
                Players =
                [
                    new Player
                    {
                        UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                        Name = "Player1",
                        Colour = Colour.Red,
                        Points = 0,
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
                    }
                ]
            },
            new HexCoordinate(1, -2),
            3
        );
        // A0 (north 2): its north edge faces (0, 0)
        Add(
            new Game
            {
                LobbyId = 3,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A0,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A0,
                Players =
                [
                    new Player
                    {
                        UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                        Name = "Player1",
                        Colour = Colour.Red,
                        Points = 0,
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
                    }
                ]
            },
            new HexCoordinate(0, 1),
            0
        );
        // the drawn tile has no cost towards (0, 0), but (0, 0) has a north cost towards the place
        Add(
            new Game
            {
                LobbyId = 4,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A0,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A0,
                Players =
                [
                    new Player
                    {
                        UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                        Name = "Player1",
                        Colour = Colour.Red,
                        Points = 0,
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
                        Costs = new TravelCosts(North: 1),
                        Coordinate = new HexCoordinate(0, 0)
                    },
                    new TempleTile
                    {
                        TempleLevel = 1,
                        Costs = new TravelCosts(NorthWest: 1),
                        Coordinate = new HexCoordinate(1, 0)
                    }
                ]
            },
            new HexCoordinate(0, -1),
            2
        );
        // A2 (south 2) turned one step clockwise: south becomes south west and faces (1, -1)
        Add(
            new Game
            {
                LobbyId = 5,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A2,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A2,
                Players =
                [
                    new Player
                    {
                        UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                        Name = "Player1",
                        Colour = Colour.Red,
                        Points = 0,
                        TurnPosition = 0
                    },
                    new Player
                    {
                        UserId = "1a6a2a19-d2a3-46eb-b732-da1550ccc8d1",
                        Name = "Player2",
                        Colour = Colour.Black,
                        Points = 0,
                        TurnPosition = 1
                    },
                    new Player
                    {
                        UserId = "5d0c1f5e-7c0b-4f0a-9a57-1c4c6b0f2f11",
                        Name = "Player3",
                        Colour = Colour.Green,
                        Points = 0,
                        TurnPosition = 2
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
                        Coordinate = new HexCoordinate(1, 0)
                    }
                ]
            },
            new HexCoordinate(2, -2),
            1
        );
        // A5 (north east 2) turned four steps clockwise: north east becomes north west and faces (1, -1)
        Add(
            new Game
            {
                LobbyId = 6,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A5,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A5,
                Players =
                [
                    new Player
                    {
                        UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                        Name = "Player1",
                        Colour = Colour.Red,
                        Points = 0,
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
                    }
                ]
            },
            new HexCoordinate(2, -1),
            4
        );
        // A3 (south 2) turned one step counter-clockwise: south becomes south east and faces (0, 0)
        Add(
            new Game
            {
                LobbyId = 7,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A3,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A3,
                Players =
                [
                    new Player
                    {
                        UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                        Name = "Player1",
                        Colour = Colour.Red,
                        Points = 0,
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
                    }
                ]
            },
            new HexCoordinate(-1, 0),
            -1
        );
    }
}