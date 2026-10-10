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
                    SelectedColour = Colour.Green,
                    IsOwner = true,
                    IsReady = true
                },
                new Player
                {
                    UserId = "ad0aa8f7-8187-4aca-88bb-a2a4ef981370",
                    SelectedColour = Colour.Yellow,
                    IsOwner = false,
                    IsReady = true
                },
                new Player
                {
                    UserId = "4aea119e-acd9-4f73-84ab-1a348910768f",
                    SelectedColour = Colour.Red,
                    IsOwner = false,
                    IsReady = true
                }
            ]
        });
    }
}