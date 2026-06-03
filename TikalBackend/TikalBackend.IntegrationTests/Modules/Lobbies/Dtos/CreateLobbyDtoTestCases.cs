using RestApi.Controllers.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

internal static class CreateLobbyDtoTestCases
{
    public static IEnumerable<CreateLobbyDto> ValidCreateLobbyDtos =>
    [
        new() { Name = "LobbyName", MaxPlayers = 2 },
        new() { Name = "MyLobby", MaxPlayers = 3 },
        new() { Name = "_]K6korI;Ij+)gXVJ].:<G&q)TxEVJ", MaxPlayers = 4 }
    ];

    public static IEnumerable<CreateLobbyDto> InvalidCreateLobbyDtos =>
    [
        // empty name
        new() { Name = "", MaxPlayers = 2 },
        new() { Name = "                  ", MaxPlayers = 3 },
        // name longer than 30 characters
        new() { Name = "6y3IQ=(~#k-9-;@V)B%BzA`5dSRbG1m", MaxPlayers = 2 },
        // maxPlayer smaller than 2
        new() { Name = "LobbyName", MaxPlayers = 1 },
        new() { Name = "1234567", MaxPlayers = -23487923 },
        // maxPlayer greater than 4
        new() { Name = "MyLobby", MaxPlayers = 5 },
        new() { Name = "Test123!", MaxPlayers = 293858934 }
    ];
}