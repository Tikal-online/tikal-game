using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal static class CreateLobbyCommandTestCases
{
    public static IEnumerable<CreateLobbyCommand> InvalidCreateLobbyCommands =>
    [
        // empty name
        new("", 2),
        // name longer than 30 characters
        new("nGBI7UZ{+grfV!a~@Q0<+?8u#[lPMOg", 2),
        // maxPlayers smaller than 2
        new("Lobby123", 1),
        new("MyLobby", -1231),
        // maxPlayers greater then 4
        new("TestLobby", 5),
        new("Hello", 3249872)
    ];

    public static IEnumerable<CreateLobbyCommand> ValidCreateLobbyCommands =>
    [
        new("LobbyName", 2),
        new("MyLobby123", 3),
        new("rX`J%trwH3=+v8HQA)]fk:dBNb,!3e", 4)
    ];
}