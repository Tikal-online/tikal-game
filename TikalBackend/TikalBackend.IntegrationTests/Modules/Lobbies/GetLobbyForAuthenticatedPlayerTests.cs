using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class GetLobbyForAuthenticatedPlayerTests : IntegrationTestFixture
{
    private const string lobbyUrl = "Lobbies";

    [Test]
    public async Task GivenUnauthenticatedUser_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsync(lobbyUrl + "/me");

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenPlayerNotInALobby_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerInALobby_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsLobby(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await Client.PostAsyncWithUser(lobbyUrl, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.Default);

        var lobby = await response.Content.ReadFromJsonAsync<LobbyDto>();

        // then
        Assert.That(lobby, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(lobby.Name, Is.EqualTo(createLobbyDto.Name));
            Assert.That(lobby.MaxPlayers, Is.EqualTo(createLobbyDto.MaxPlayers));

            Assert.That(lobby.Players, Has.Count.EqualTo(1));
            Assert.That(lobby.Players[0].UserId, Is.EqualTo(TestUser.Default.UserId));
            Assert.That(lobby.Players[0].Name, Is.EqualTo(TestUser.Default.Name));
        }
    }
}