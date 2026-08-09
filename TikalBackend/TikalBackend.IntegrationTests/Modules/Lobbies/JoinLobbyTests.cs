using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class JoinLobbyTests : IntegrationTestFixture
{
    [Test]
    public async Task GivenUnauthenticatedUser_WhenJoinLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsync(LobbyUrl.JoinLobby(1), null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenJoinLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(1), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenNoLobbyWithId_WhenJoinLobby_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(1), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerAlreadyInLobby_WhenJoinLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.TestUser1, createLobbyDto);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.TestUser1);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby!.Id), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenFullLobby_WhenJoinLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.TestUser1, createLobbyDto);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.TestUser1);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        List<TestUser> users = [TestUser.TestUser2, TestUser.TestUser3, TestUser.TestUser4];

        // players join until the lobby is full
        for (var i = 0; i < lobby!.MaxPlayers - 1; i++)
        {
            await CreateUserAccount(users[i]);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), users[i], null);
        }

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }
}