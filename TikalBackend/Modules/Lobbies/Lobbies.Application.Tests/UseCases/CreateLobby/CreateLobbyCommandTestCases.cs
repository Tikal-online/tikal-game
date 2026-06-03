using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal static class CreateLobbyCommandTestCases
{
    public static IEnumerable<CreateLobbyCommand> InvalidCreateLobbyCommands =>
    [
        // empty user id
        new("", "LobbyName", 4),
        // empty name
        new("7b5e157a-0a6a-41f5-8e15-acce253f2494", "", 2),
        // user id longer than 100 characters
        new("_!=4:NZ(d464`bk;#1b;3pM^/VENf8AP`<Q8#6~KWj)PC5.XBfXYYO9^b/s9f=#y;XUW@orMyTmU:Rgj8m`6G~C1@8AMC9mr+*_KR",
            "LobbyName",
            3),
        // name longer than 30 characters
        new("7b5e157a-0a6a-41f5-8e15-acce253f2494", "nGBI7UZ{+grfV!a~@Q0<+?8u#[lPMOg", 2),
        // maxPlayers smaller than 2
        new("11a17341-145d-4e21-bc19-a987a4b68c45", "Lobby123", 1),
        new("44c2d5a9-de34-4016-93e1-fc534cfe2920", "MyLobby", -1231),
        // maxPlayers greater then 4
        new("688b442c-3dbe-4c2e-b725-ab5df9b1b775", "TestLobby", 5),
        new("e247b639-8fa9-4af8-afcc-aa5097636ad8", "Hello", 3249872)
    ];

    public static IEnumerable<CreateLobbyCommand> ValidCreateLobbyCommands =>
    [
        new("fe74b6f2-96df-4acf-b91a-38a95f434081", "LobbyName", 2),
        new("2c5d4835-ef41-47da-8b5e-e7bbccdd27bb", "MyLobby123", 3),
        new("4bfa38ff-f0f2-4363-ae0c-704fa71266fb", "rX`J%trwH3=+v8HQA)]fk:dBNb,!3e", 4)
    ];
}