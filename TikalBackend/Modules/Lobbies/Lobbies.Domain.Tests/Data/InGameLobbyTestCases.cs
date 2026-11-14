using Lobbies.Domain.Entities;
using Shared.Domain.Enums;

namespace Lobbies.Domain.Tests.Data;

public class InGameLobbies : TheoryData<Lobby>
{
    public InGameLobbies()
    {
        Add(new Lobby()
        {
            Id = 5,
            MaxPlayers = 3,
            Name = "InGameLobby",
            InGame = true,
            Players =
            [
                new Player
                {
                    UserId = "db3e91e9-71d1-40d1-a8f6-516d803fd28e",
                    Name = "Player1",
                    SelectedColour = Colour.Green,
                    IsOwner = true,
                    IsReady = true
                },
                new Player
                {
                    UserId = "ad0aa8f7-8187-4aca-88bb-a2a4ef981370",
                    Name = "Player2",
                    SelectedColour = Colour.Yellow,
                    IsOwner = false,
                    IsReady = true
                },
                new Player
                {
                    UserId = "4aea119e-acd9-4f73-84ab-1a348910768f",
                    Name = "Player3",
                    SelectedColour = Colour.Red,
                    IsOwner = false,
                    IsReady = true
                }
            ]
        }.WithLinkedPlayers());

        Add(new Lobby()
        {
            Id = 6,
            MaxPlayers = 2,
            Name = "InGameDuo",
            InGame = true,
            Players =
            [
                new Player
                {
                    UserId = "708edb1c-3385-4e13-ab4a-623b649ec21b",
                    Name = "Player1",
                    SelectedColour = Colour.Blue,
                    IsOwner = true,
                    IsReady = true
                },
                new Player
                {
                    UserId = "a36a7130-abde-45e3-9cd7-70a8565eb0ff",
                    Name = "Player2",
                    SelectedColour = Colour.Red,
                    IsOwner = false,
                    IsReady = true
                }
            ]
        }.WithLinkedPlayers());

        Add(new Lobby()
        {
            Id = 7,
            MaxPlayers = 4,
            Name = "InGameHalfFull",
            InGame = true,
            Players =
            [
                new Player
                {
                    UserId = "2b0a5b99-0a24-4180-b942-2023faf24997",
                    Name = "Player1",
                    SelectedColour = Colour.Black,
                    IsOwner = false,
                    IsReady = true
                },
                new Player
                {
                    UserId = "b7cf0b0b-a3b4-4f11-b861-e7d65a5acb88",
                    Name = "Player2",
                    SelectedColour = Colour.Yellow,
                    IsOwner = true,
                    IsReady = true
                }
            ]
        }.WithLinkedPlayers());

        Add(new Lobby()
        {
            Id = 8,
            MaxPlayers = 4,
            Name = "InGameFull",
            InGame = true,
            Players =
            [
                new Player
                {
                    UserId = "a2f81f55-8706-47a8-b1d4-4d6ee68003ea",
                    Name = "Player1",
                    SelectedColour = Colour.Red,
                    IsOwner = true,
                    IsReady = true
                },
                new Player
                {
                    UserId = "4b704d05-7599-457c-8f49-33a36d946b9d",
                    Name = "Player2",
                    SelectedColour = Colour.Black,
                    IsOwner = false,
                    IsReady = true
                },
                new Player
                {
                    UserId = "ead2042c-a6f1-48b2-bfb0-a67343f4aeb7",
                    Name = "Player3",
                    SelectedColour = Colour.Green,
                    IsOwner = false,
                    IsReady = true
                },
                new Player
                {
                    UserId = "bd7fb54c-4205-4ba9-a602-247f229ec320",
                    Name = "Player4",
                    SelectedColour = Colour.Blue,
                    IsOwner = false,
                    IsReady = true
                }
            ]
        }.WithLinkedPlayers());
    }
}