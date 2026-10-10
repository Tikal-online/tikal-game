using RestApi.Controllers.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

internal class InvalidCreateLobbyDtos : TheoryData<CreateLobbyDto>
{
    public InvalidCreateLobbyDtos()
    {
        // empty name
        Add(new CreateLobbyDto() { Name = "", MaxPlayers = 2 });
        Add(new CreateLobbyDto() { Name = "        ", MaxPlayers = 3 });
        // name longer then 30 characters
        Add(new CreateLobbyDto() { Name = "6y3IQ=(~#k-9-;@V)B%BzA`5dSRbG1m", MaxPlayers = 2 });
        // maxPlayers smaller then 2
        Add(new CreateLobbyDto() { Name = "LobbyName", MaxPlayers = 1 });
        Add(new CreateLobbyDto() { Name = "1234567", MaxPlayers = -4320982 });
        // maxPlayers greater then 4
        Add(new CreateLobbyDto() { Name = "MyLobby", MaxPlayers = 5 });
        Add(new CreateLobbyDto() { Name = "Test123!", MaxPlayers = 293842990 });
    }
}