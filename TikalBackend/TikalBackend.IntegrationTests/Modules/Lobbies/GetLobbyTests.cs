using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;
using Xunit;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class GetLobbyTests : IntegrationTestFixture
{
    public static IEnumerable<long> LobbyIdTestCases =>
    [
        0,
        1,
        123,
        45345,
        3450934853
    ];

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
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetLobby(lobbyId), TestUser.Default);

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
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetLobby(lobbyId), TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenLobbyWithId_WhenGetLobby_ThenReturnsLobby(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        var createdLobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);

        var createdLobby = await createdLobbyResponse.Content.ReadFromJsonAsync<LobbyDto>(TestContext.Current.CancellationToken);

        // when
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetLobby(createdLobby!.Id), TestUser.Default);

        var lobby = await response.Content.ReadFromJsonAsync<LobbyDto>(TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(lobby);

        // TODO: assertions
        /*
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(lobby.Id, Is.EqualTo(createdLobby.Id));
            Assert.That(lobby.Name, Is.EqualTo(createdLobby.Name));
            Assert.That(lobby.MaxPlayers, Is.EqualTo(createdLobby.MaxPlayers));
            Assert.That(lobby.InGame, Is.EqualTo(createdLobby.InGame));

            Assert.That(lobby.Players, Is.EquivalentTo(createdLobby.Players));
        }
        */
    }
}