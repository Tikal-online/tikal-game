using System.Net;
using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Lobbies.Dtos;
using SignalRApi.Hubs.Games.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class ActiveGameTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenConnect_ThenThrowsInvalidOperation()
    {
        // when & then
        return Assert.ThrowsAsync<InvalidOperationException>(() =>
        {
            return CreateConnection(GameUrl.ActiveGameHub);
        });
    }

    [Fact]
    public async Task GivenuserWithoutAccount_WhenConnect_ThenThrowsAccountRequiredHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.Default, false);
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
    public async Task GivenUserNotInAGame_WhenConnect_ThenThrowsNotInAGameHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await CreateUserAccount(TestUser.Default);
        await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.Default, false);
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
        Assert.Contains("Player is not in a game", exception.Message);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenGame_WhenOnePlayerLeaves_ThenSendsPlayerLeftMessage(CreateLobbyDto createLobbyDto)
    {
        // given

        // create and start the lobby
        await RunTracked(async () =>
        {
            await CreateUserAccount(TestUser.Default);
            await CreateUserAccount(TestUser.TestUser1);

            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);
            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);
        });

        await RunTracked(async () =>
        {
            await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.Default);

            var leftPlayerSource = new TaskCompletionSource<GamePlayerDto>();
            connection.On<GamePlayerDto>("PlayerLeft", leftPlayerSource.SetResult);

            // when
            var game = await GetActiveGame(TestUser.Default);

            await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(game.Id), TestUser.TestUser1, TestContext.Current.CancellationToken);

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
    public async Task GivenGameWith2Players_WhenOnePlayerLeaves_ThenSendsGameCompletedMessage(CreateLobbyDto createLobbyDto)
    {
        // given

        // create and start the lobby
        await RunTracked(async () =>
        {
            await CreateUserAccount(TestUser.Default);
            await CreateUserAccount(TestUser.TestUser1);

            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);
            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);
        });

        await RunTracked(async () =>
        {
            await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.Default);

            var gameCompletedSource = new TaskCompletionSource();
            connection.On("GameCompleted", gameCompletedSource.SetResult);

            // when
            var game = await GetActiveGame(TestUser.Default);

            var response = await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(game.Id), TestUser.TestUser1, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            await gameCompletedSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        });
    }
}