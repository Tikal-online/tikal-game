using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class LeaveLobbyTests : IntegrationTestFixture
{
    private const string lobbyUrl = "Lobbies";

    private static string BuildUrl(long id)
    {
        return $"Lobbies/{id}/players/me";
    }

    [Test]
    public async Task GivenUnauthenticatedUser_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsync(BuildUrl(1));

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsyncWithUser(BuildUrl(1), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenLobbyDoesntExists_WhenLeaveLobby_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.DeleteAsyncWithUser(BuildUrl(1), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotInLobby_WhenLeaveLobby_ThenReturnsNotFound(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        await Client.PostAsyncWithUser(lobbyUrl, TestUser.TestUser1, createLobbyDto);
        var lobbyResponse = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.TestUser1);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        var response = await Client.DeleteAsyncWithUser(BuildUrl(lobby!.Id), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerInLobby_WhenLeaveLobby_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await Client.PostAsyncWithUser(lobbyUrl, TestUser.Default, createLobbyDto);
        var lobbyResponse = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        var response = await Client.DeleteAsyncWithUser(BuildUrl(lobby!.Id), TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}