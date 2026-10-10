using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class GetLobbyTests : IntegrationTestFixture
{
    [Theory]
    [ClassData(typeof(LobbyIdTestCases))]
    public async Task GivenUnauthenticatedUser_WhenGetLobby_ThenReturnsUnauthorized(long lobbyId)
    {
        // when
        var response = await Client.GetAsync(LobbyUrl.GetLobby(lobbyId), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(LobbyIdTestCases))]
    public async Task GivenUserWithoutAccount_WhenGetLobby_ThenReturnsUnauthorized(long lobbyId)
    {
        // when
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetLobby(lobbyId), TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(LobbyIdTestCases))]
    public async Task GivenNoLobbyWithId_WhenGetLobby_ThenReturnsNotFound(long lobbyId)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetLobby(lobbyId), TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenLobbyWithId_WhenGetLobby_ThenReturnsLobby(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

        var createdLobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default, TestContext.Current.CancellationToken);

        var createdLobby = await createdLobbyResponse.Content.ReadFromJsonAsync<LobbyDto>(TestContext.Current.CancellationToken);

        // when
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetLobby(createdLobby!.Id), TestUser.Default, TestContext.Current.CancellationToken);

        var lobby = await response.Content.ReadFromJsonAsync<LobbyDto>(TestContext.Current.CancellationToken);

        // then
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.NotNull(lobby),
            () => Assert.Equal(createdLobby.Id, lobby?.Id),
            () => Assert.Equal(createdLobby.Name, lobby?.Name),
            () => Assert.Equal(createdLobby.MaxPlayers, lobby?.MaxPlayers),
            () => Assert.Equal(createdLobby.InGame, lobby?.InGame),
            () => Assert.Equal(createdLobby.Players, lobby?.Players)
        );
    }
}