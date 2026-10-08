using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Lobbies.Dtos;
using SignalRApi.Hubs.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts.Dtos;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;
using Xunit;
using LobbyPlayerDto = SignalRApi.Hubs.Lobbies.Dtos.LobbyPlayerDto;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class ActiveLobbyTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenConnect_ThenReturnsUnauthorized()
    {
        // when & then
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await CreateConnection(LobbyUrl.ActiveLobbyHub);
        });

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenConnect_ThenThrowsAccountRequiredHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default, false);
        connection.Closed += ex =>
        {
            closedExceptionSource.TrySetResult(ex);
            return Task.CompletedTask;
        };

        // when
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // then
        var exception = await closedExceptionSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.NotNull(exception);
        Assert.Contains(exception.Message, "Account required");
    }

    [Fact]
    public async Task GivenUserNotInALobby_WhenConnect_ThenThrowsNotInALobbyHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await CreateUserAccount(TestUser.Default);
        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default, false);
        connection.Closed += ex =>
        {
            closedExceptionSource.TrySetResult(ex);
            return Task.CompletedTask;
        };

        // when
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // then
        var exception = await closedExceptionSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.NotNull(exception);
        Assert.Contains(exception.Message, "Player is not in a lobby");
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenLobby_WhenPlayerJoinsLobby_ThenSendsPlayerJoinedMessage(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);
        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

        var joinedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
        connection.On<LobbyPlayerDto>("PlayerJoined", joinedPlayerSource.SetResult);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>(TestContext.Current.CancellationToken);

        // when
        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby!.Id), TestUser.TestUser1, null);

        // then
        var joinedPlayer = await joinedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.NotNull(joinedPlayer);

        // TODO: do assertions
        /*
        using (Assert.EnterMultipleScope())
        {
            Assert.That(joinedPlayer.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
            Assert.That(joinedPlayer.Name, Is.EqualTo(TestUser.TestUser1.Name));
        }
        */
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenLobby_WhenPlayerLeavesLobby_ThenSendsPlayerLeftMessage(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);
        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

        var leftPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
        connection.On<LobbyPlayerDto>("PlayerLeft", leftPlayerSource.SetResult);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>(TestContext.Current.CancellationToken);

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby!.Id), TestUser.TestUser1, null);

        // when
        await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.TestUser1);

        // then
        var leftPlayer = await leftPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.NotNull(leftPlayer);

        // TODO: do assertions
        /*
        using (Assert.EnterMultipleScope())
        {
            Assert.That(leftPlayer.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
            Assert.That(leftPlayer.Name, Is.EqualTo(TestUser.TestUser1.Name));
        }
        */
    }

    /*
    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenLobby_WhenLastOwnerLeavesLobby_ThenPromotesPlayerAndSendsPlayerUpdatedNotification(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby!.Id), TestUser.TestUser1, null);

        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.TestUser1);
        await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.TestUser1);

        var updatedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
        connection.On<LobbyPlayerDto>("PlayerUpdated", updatedPlayerSource.SetResult);

        // when
        await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.Default);

        // then
        var updatedPlayer = await updatedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(updatedPlayer, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedPlayer.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
            Assert.That(updatedPlayer.Name, Is.EqualTo(TestUser.TestUser1.Name));
            Assert.That(updatedPlayer.IsOwner, Is.True);
        }
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenLobby_WhenPlayerSendsChatMessage_ThenSendsChatMessage(SendMessageDto sendMessageDto)
    {
        // given
        var createLobbyDto = new CreateLobbyDto
        {
            Name = "Test Lobby",
            MaxPlayers = 4
        };

        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);
        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

        var chatMessageSource = new TaskCompletionSource<ChatMessageDto>();
        connection.On<ChatMessageDto>("ReceiveMessage", chatMessageSource.SetResult);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby!.Id), TestUser.TestUser1, null);

        // when
        await Client.PostAsyncWithUser(LobbyUrl.SendMessage(lobby.Id), TestUser.TestUser1, sendMessageDto);

        // then
        var chatMessage = await chatMessageSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(chatMessage, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(chatMessage.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
            Assert.That(chatMessage.Username, Is.EqualTo(TestUser.TestUser1.Name));
            Assert.That(chatMessage.Content, Is.EqualTo(sendMessageDto.Message));
        }
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenLobby_WhenPlayerReadyUp_ThenSetsPlayerToReadyAndSendsPlayerUpdatedNotification(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);
        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby!.Id), TestUser.TestUser1, null);

        var updatedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
        connection.On<LobbyPlayerDto>("PlayerUpdated", updatedPlayerSource.SetResult);

        // when
        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null);

        // then
        var updatedPlayer = await updatedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(updatedPlayer, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedPlayer.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
            Assert.That(updatedPlayer.Name, Is.EqualTo(TestUser.TestUser1.Name));
            Assert.That(updatedPlayer.IsReady, Is.True);
        }
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenLobby_WhenPlayerReadyDown_ThenSetsPlayerToNotReadyAndSendsPlayerUpdatedNotification(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);
        await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

        var lobbyResponse = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby!.Id), TestUser.TestUser1, null);
        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null);

        var updatedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
        connection.On<LobbyPlayerDto>("PlayerUpdated", updatedPlayerSource.SetResult);

        // when
        await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.TestUser1);

        // then
        var updatedPlayer = await updatedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(updatedPlayer, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedPlayer.UserId, Is.EqualTo(TestUser.TestUser1.UserId));
            Assert.That(updatedPlayer.Name, Is.EqualTo(TestUser.TestUser1.Name));
            Assert.That(updatedPlayer.IsReady, Is.False);
        }
    }
    */
}