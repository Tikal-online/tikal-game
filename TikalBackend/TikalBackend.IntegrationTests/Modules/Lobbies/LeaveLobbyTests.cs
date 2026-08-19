using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class LeaveLobbyTests : IntegrationTestFixture
{
    [Test]
    public async Task GivenUnauthenticatedUser_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsync(LobbyUrl.LeaveLobby(1));

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(1), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenLobbyDoesntExists_WhenLeaveLobby_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(1), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotInLobby_WhenLeaveLobby_ThenReturnsNotFound(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.TestUser1, createLobbyDto);
        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.TestUser1);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby!.Id), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerInLobby_WhenLeaveLobby_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);
        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby!.Id), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenLobbyInGame_WhenLeaveLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null);

        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null);
        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null);

        await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }
}