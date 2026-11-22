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

    [Theory]
    [ClassData(typeof(CreateLobbyDtoWith3orMorePlayers))]
    public async Task GivenGame_WhenCurrentPlayerLeaves_ThenSendsTurnCompletedMessage(CreateLobbyDto createLobbyDto)
    {
        // given

        // create and start the lobby
        await RunTracked(async () =>
        {
            await CreateUserAccount(TestUser.Default);
            await CreateUserAccount(TestUser.TestUser1);
            await CreateUserAccount(TestUser.TestUser2);

            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser2, null, TestContext.Current.CancellationToken);

            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);
            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);
            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser2, null, TestContext.Current.CancellationToken);

            await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);
        });

        await RunTracked(async () =>
        {
            await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.TestUser1);

            var turnCompletedSource = new TaskCompletionSource<GamePlayerDto>();
            connection.On<GamePlayerDto>("TurnCompleted", turnCompletedSource.SetResult);

            var game = await GetActiveGame(TestUser.Default);
            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1, TestUser.TestUser2];
            var currentUser = users.Single(u => u.UserId == currentPlayer.UserId);

            // when
            var response = await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(game.Id), currentUser, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            await turnCompletedSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenGame_WhenTilePlaced_ThenSendsTilePlacedMessage(CreateLobbyDto createLobbyDto)
    {
        // given
        var dto = new RestApi.Controllers.Games.Dtos.PlaceTileDto() { Q = 1, R = -2, Rotation = 2 };

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
            await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.TestUser1);

            var tilePlacedSource = new TaskCompletionSource<TileDto>();
            connection.On<TileDto>("TilePlaced", tilePlacedSource.SetResult);

            var game = await GetActiveGame(TestUser.Default);
            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var currentUser = users.First(u => u.UserId == currentPlayer.UserId);

            await Client.GetAsyncWithUser(GameUrl.GetNextTile(game.Id), currentUser, TestContext.Current.CancellationToken);

            // when
            await Client.PostAsyncWithUser(GameUrl.PlaceTile(game.Id), currentUser, dto, TestContext.Current.CancellationToken);

            // then
            var tile = await tilePlacedSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            Assert.Multiple(
                () => Assert.NotNull(tile),
                () => Assert.Equal(tile.Coordinate.Q, dto.Q),
                () => Assert.Equal(tile.Coordinate.R, dto.R)
            );
        });
    }
}