using RestApi.Controllers.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

internal class CreateLobbyDtoWith3orMorePlayers : TheoryData<CreateLobbyDto>
{
    public CreateLobbyDtoWith3orMorePlayers()
    {
        Add(new CreateLobbyDto() { Name = "MyLobby", MaxPlayers = 3 });
        Add(new CreateLobbyDto() { Name = "_]K6korI;Ij+)gXVJ].:<G&q)TxEVJ", MaxPlayers = 4 });
    }
}