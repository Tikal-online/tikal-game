
using RestApi.Controllers.Lobbies.Dtos;
using Xunit;

namespace TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

internal class ValidCreateLobbyDtos : TheoryData<CreateLobbyDto>
{
    public ValidCreateLobbyDtos()
    {
        Add(new CreateLobbyDto() { Name = "LobbyName", MaxPlayers = 2 });
        Add(new CreateLobbyDto() { Name = "MyLobby", MaxPlayers = 3 });
        Add(new CreateLobbyDto() { Name = "_]K6korI;Ij+)gXVJ].:<G&q)TxEVJ", MaxPlayers = 4 });
    }
}