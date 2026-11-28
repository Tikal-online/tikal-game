using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class LeaveGameTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenLeaveGame_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.DeleteAsync(GameUrl.LeaveGame(1), TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenUserWithoutAccount_WhenLeaveGame_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(1), TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenGameDoesntExist_WhenLeaveGame_ThenReturnsNotFound()
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(1), TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotInGame_WhenLeaveGame_ThenReturnsNotFound(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await CreateUserAccount(TestUser.TestUser2);

        await RunTracked(async () =>
        {
            // when
            var game = await GetActiveGame(TestUser.Default);

            var response = await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(game.Id), TestUser.TestUser2, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerInGame_WhenLeaveGame_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            // when
            var game = await GetActiveGame(TestUser.Default);

            var response = await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(game.Id), TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(CreateLobbyDtoWith3orMorePlayers))]
    public async Task GivenGame_WhenCurrentPlayerLeaves_ThenEndsPlayerTurn(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1, TestUser.TestUser2], createLobbyDto);

        await RunTracked(async () =>
        {
            // when
            var gameBefore = await GetActiveGame(TestUser.Default);

            var currentPlayer = gameBefore.Players.Single(p => p.TurnPosition == gameBefore.CurrentTurnPosition);

            List<TestUser> users = [TestUser.Default, TestUser.TestUser1, TestUser.TestUser2];

            var currentUser = users.Single(u => u.UserId == currentPlayer.UserId);

            await Client.DeleteAsyncWithUser(GameUrl.LeaveGame(gameBefore.Id), currentUser, TestContext.Current.CancellationToken);

            var inGameUser = users.First(u => u.UserId != currentPlayer.UserId);

            var gameAfter = await GetActiveGame(inGameUser);

            // then
            Assert.NotEqual(gameBefore.CurrentTurnPosition, gameAfter.CurrentTurnPosition);
        });
    }
}