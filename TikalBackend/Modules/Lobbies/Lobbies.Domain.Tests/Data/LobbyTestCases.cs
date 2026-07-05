using Lobbies.Domain.Entities;
using Lobbies.Domain.Enums;

namespace Lobbies.Domain.Tests.Data;

public static class LobbyTestCases
{
    public static IEnumerable<Lobby> ValidLobbyTestCases =>
    [
        new()
        {
            Id = 1,
            MaxPlayers = 2,
            Name = "LobbyName",
            Players =
            [
                new Player
                {
                    UserId = "595b266b-7b12-4e3e-a83a-646ca364e1a7",
                    SelectedColour = Colour.Black,
                    IsOwner = true,
                    IsReady = false
                }
            ]
        },
        new()
        {
            Id = 2,
            MaxPlayers = 3,
            Name = "TestLobby123",
            Players =
            [
                new Player
                {
                    UserId = "ab6e7381-1570-4cbb-a149-00c34e33499e",
                    SelectedColour = Colour.Red,
                    IsOwner = true,
                    IsReady = false
                },
                new Player
                {
                    UserId = "e369a3fb-47d2-4f7e-b2d2-3a2cecbd0b91",
                    SelectedColour = Colour.Black,
                    IsOwner = false,
                    IsReady = false
                }
            ]
        },
        new()
        {
            Id = 3,
            MaxPlayers = 4,
            Name = "234cs0dj234jnkls",
            Players =
            [
                new Player
                {
                    UserId = "11875b18-8577-43a4-9685-9c4b00d3f5a1",
                    SelectedColour = Colour.Red,
                    IsOwner = true,
                    IsReady = true
                },
                new Player
                {
                    UserId = "f4c62607-e16d-47e0-801d-6d77c454abea",
                    SelectedColour = Colour.Black,
                    IsOwner = false,
                    IsReady = true
                },
                new Player
                {
                    UserId = "40023ae5-63d9-48ca-b8f3-bccaa948a6e7",
                    SelectedColour = Colour.Yellow,
                    IsOwner = true,
                    IsReady = false
                }
            ]
        }
    ];
}