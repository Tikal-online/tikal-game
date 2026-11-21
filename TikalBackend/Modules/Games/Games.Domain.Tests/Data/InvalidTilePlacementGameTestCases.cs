using Games.Domain.Entities;
using Games.Domain.Enums;
using Games.Domain.Types;
using Shared.Domain.Enums;

namespace Games.Domain.Tests.Data;

// the test cases follow the structure: game with a drawn tile, location, rotation
internal class InvalidTilePlacementGames : TheoryData<Game, HexCoordinate, int>
{
    public InvalidTilePlacementGames()
    {
        // A0 has no cost on its south edge and (1, -1) none on its north edge
        Add(
            new Game
            {
                LobbyId = 1,
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
            0
        );
        // A1 is valid here unturned, turned half way its south cost faces away from (1, -1)
        Add(
            new Game
            {
                LobbyId = 2,
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
            3
        );
        // A5 has no cost on its north west edge and (1, -1) none on its south east edge
        Add(
            new Game
            {
                LobbyId = 3,
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
            0
        );
        // A4 has no cost on its north east edge and (0, 0) none on its south west edge
        Add(
            new Game
            {
                LobbyId = 4,
                TurnPhase = TurnPhase.PlaceTile,
                PendingTile = TileCardIdentifier.A4,
                DrawnTiles = 1UL << (int)TileCardIdentifier.A4,
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
            new HexCoordinate(-1, 1),
            0
        );
        // (0, 0) has costs, but on edges facing away from the place
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
                    }
                ],
                Tiles =
                [
                    new EmptyTile
                    {
                        Costs = new TravelCosts(North: 1, SouthEast: 1),
                        Coordinate = new HexCoordinate(0, 0)
                    }
                ]
            },
            new HexCoordinate(-1, 1),
            0
        );
        // no neighbouring tile at all
        Add(
            new Game
            {
                LobbyId = 6,
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
            new HexCoordinate(3, -4),
            0
        );
        // the place is already taken
        Add(
            new Game
            {
                LobbyId = 7,
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
                    }
                ]
            },
            new HexCoordinate(0, 0),
            0
        );
    }
}