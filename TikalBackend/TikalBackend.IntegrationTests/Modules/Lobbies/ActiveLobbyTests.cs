using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;
using LobbyPlayerDto = SignalRApi.Hubs.Lobbies.Dtos.LobbyPlayerDto;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class ActiveLobbyTests : IntegrationTestFixture
{
    private const string activeLobbyUrl = "hub/activeLobby";

    private const string lobbyUrl = "Lobbies";

    [Test]
    public void GivenUnauthenticatedUser_WhenConnect_ThenReturnsUnauthorized()
    {
        // when & then
        var exception =
            Assert.ThrowsAsync<HttpRequestException>(async () => { await CreateConnection(activeLobbyUrl); });

        Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenConnect_ThenThrowsAccountRequiredHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await using var connection = await CreateConnection(activeLobbyUrl, TestUser.Default, false);
        connection.Closed += ex =>
        {
            closedExceptionSource.TrySetResult(ex);
            return Task.CompletedTask;
        };

        // when
        await connection.StartAsync();

        // then
        var exception = await closedExceptionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Does.Contain("Account required"));
    }

    [Test]
    public async Task GivenUserNotInALobby_WhenConnect_ThenThrowsNotInALobbyHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await CreateUserAccount(TestUser.Default);
        await using var connection = await CreateConnection(activeLobbyUrl, TestUser.Default, false);
        connection.Closed += ex =>
        {
            closedExceptionSource.TrySetResult(ex);
            return Task.CompletedTask;
        };

        // when
        await connection.StartAsync();

        // then
        var exception = await closedExceptionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Does.Contain("Player is not in a lobby"));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenLobby_WhenPlayerJoinsLobby_ThenSendsPlayerJoinedMessage(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(lobbyUrl, TestUser.Default, createLobbyDto);
        await using var connection = await CreateConnection(activeLobbyUrl, TestUser.Default);

        var joinedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
        connection.On<LobbyPlayerDto>("PlayerJoined", joinedPlayerSource.SetResult);

        var lobbyResponse = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser($"Lobbies/{lobby!.Id}/join", TestUser.TestUser1, null);

        // then
        var joinedPlayer = await joinedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(joinedPlayer, Is.Not.Null);
        Assert.That(joinedPlayer.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
        Assert.That(joinedPlayer.Name, Is.EqualTo(TestUser.TestUser1.Name));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenLobby_WhenPlayerLeavesLobby_ThenSendsPlayerLeftMessage(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(lobbyUrl, TestUser.Default, createLobbyDto);
        await using var connection = await CreateConnection(activeLobbyUrl, TestUser.Default);

        var leftPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
        connection.On<LobbyPlayerDto>("PlayerLeft", leftPlayerSource.SetResult);

        var lobbyResponse = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser($"Lobbies/{lobby!.Id}/join", TestUser.TestUser1, null);

        // when
        await Client.PostAsyncWithUser(lobbyUrl + "/leave", TestUser.TestUser1, null);

        // then
        var leftPlayer = await leftPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(leftPlayer, Is.Not.Null);
        Assert.That(leftPlayer.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
        Assert.That(leftPlayer.Name, Is.EqualTo(TestUser.TestUser1.Name));
    }
}