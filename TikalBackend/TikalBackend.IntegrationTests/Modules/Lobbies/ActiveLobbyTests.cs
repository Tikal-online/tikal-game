using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;
using LobbyPlayerDto = SignalRApi.Hubs.Lobbies.Dtos.LobbyPlayerDto;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class ActiveLobbyTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenConnect_ThenThrowsInvalidOperation()
    {
        // when & then
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await CreateConnection(LobbyUrl.ActiveLobbyHub);
        });
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
        Assert.Contains("Account required", exception.Message);
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
        Assert.Contains("Player is not in a lobby", exception.Message);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenLobby_WhenPlayerJoinsLobby_ThenSendsPlayerJoinedMessage(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);
            await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

            var joinedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
            connection.On<LobbyPlayerDto>("PlayerJoined", joinedPlayerSource.SetResult);

            // when
            await CreateUserAccount(TestUser.TestUser1);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            // then
            var joinedPlayer = await joinedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            Assert.Multiple(
                () => Assert.NotNull(joinedPlayer),
                () => Assert.Equal(TestUser.TestUser1.UserId, joinedPlayer.UserId),
                () => Assert.Equal(TestUser.TestUser1.Name, joinedPlayer.Name)
            );
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenLobby_WhenPlayerLeavesLobby_ThenSendsPlayerLeftMessage(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);
            await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

            var leftPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
            connection.On<LobbyPlayerDto>("PlayerLeft", leftPlayerSource.SetResult);

            await CreateUserAccount(TestUser.TestUser1);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            // when
            await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.TestUser1, TestContext.Current.CancellationToken);

            // then
            var leftPlayer = await leftPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            Assert.Multiple(
                () => Assert.NotNull(leftPlayer),
                () => Assert.Equal(TestUser.TestUser1.UserId, leftPlayer.UserId),
                () => Assert.Equal(TestUser.TestUser1.Name, leftPlayer.Name)
            );
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenLobby_WhenLastOwnerLeavesLobby_ThenPromotesPlayerAndSendsPlayerUpdatedNotification(
        CreateLobbyDto createLobbyDto
    )
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

            await CreateUserAccount(TestUser.TestUser1);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.TestUser1);
            await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.TestUser1, TestContext.Current.CancellationToken);

            var updatedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
            connection.On<LobbyPlayerDto>("PlayerUpdated", updatedPlayerSource.SetResult);

            // when
            await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.Default, TestContext.Current.CancellationToken);

            // then
            var updatedPlayer = await updatedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            Assert.Multiple(
                () => Assert.NotNull(updatedPlayer),
                () => Assert.Equal(TestUser.TestUser1.UserId, updatedPlayer.UserId),
                () => Assert.Equal(TestUser.TestUser1.Name, updatedPlayer.Name),
                () => Assert.True(updatedPlayer.IsOwner)
            );
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenLobby_WhenPlayerReadyUp_ThenSetsPlayerToReadyAndSendsPlayerUpdatedNotification(
        CreateLobbyDto createLobbyDto
    )
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);
            await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

            var updatedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
            connection.On<LobbyPlayerDto>("PlayerUpdated", updatedPlayerSource.SetResult);

            await CreateUserAccount(TestUser.TestUser1);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            // when
            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            // then
            var updatedPlayer = await updatedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            Assert.Multiple(
                () => Assert.NotNull(updatedPlayer),
                () => Assert.Equal(TestUser.TestUser1.UserId, updatedPlayer.UserId),
                () => Assert.Equal(TestUser.TestUser1.Name, updatedPlayer.Name),
                () => Assert.True(updatedPlayer.IsReady)
            );
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenLobby_WhenPlayerReadyDown_ThenSetsPlayerToNotReadyAndSendsPlayerUpdatedNotification(
        CreateLobbyDto createLobbyDto
    )
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);
            await using var connection = await CreateConnection(LobbyUrl.ActiveLobbyHub, TestUser.Default);

            await CreateUserAccount(TestUser.TestUser1);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            var firstUpdate = new TaskCompletionSource();
            connection.On<LobbyPlayerDto>("PlayerUpdated", _ => firstUpdate.SetResult());

            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            await firstUpdate.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            var updatedPlayerSource = new TaskCompletionSource<LobbyPlayerDto>();
            connection.On<LobbyPlayerDto>("PlayerUpdated", updatedPlayerSource.SetResult);

            // when
            await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.TestUser1, TestContext.Current.CancellationToken);

            // then
            var updatedPlayer = await updatedPlayerSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            Assert.Multiple(
                () => Assert.NotNull(updatedPlayer),
                () => Assert.Equal(TestUser.TestUser1.UserId, updatedPlayer.UserId),
                () => Assert.Equal(TestUser.TestUser1.Name, updatedPlayer.Name),
                () => Assert.False(updatedPlayer.IsReady)
            );
        });
    }
}