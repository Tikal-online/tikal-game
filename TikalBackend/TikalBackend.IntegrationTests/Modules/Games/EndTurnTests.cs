using System.Net;
using RestApi.Controllers.Games.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class EndTurnTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenEndTurn_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsync(GameUrl.EndTurn(1), null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenUserWithoutAccount_WhenEndTurn_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(1), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenNonExistantGame_WhenEndTurn_ThenReturnsNotFound()
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(1), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotInGame_WhenEndTurn_ThenReturnsForbidden(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await CreateUserAccount(TestUser.TestUser2);

        await RunTracked(async () =>
        {
            var game = await GetActiveGame(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(game.Id), TestUser.TestUser2, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotCurrentPlayer_WhenEndTurn_ThenReturnsForbidden(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            var game = await GetActiveGame(TestUser.Default);

            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var notCurrentUser = users.First(u => u.UserId != currentPlayer.UserId);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(game.Id), notCurrentUser, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenGameInPlaceTilePhase_WhenEndTurn_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            var game = await GetActiveGame(TestUser.Default);

            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var currentUser = users.First(u => u.UserId == currentPlayer.UserId);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(game.Id), currentUser, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenGameInActionPhase_WhenEndTurn_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        var placeTileDto = new PlaceTileDto() { Q = 1, R = -2, Rotation = 2 };

        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            var game = await GetActiveGame(TestUser.Default);

            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var currentUser = users.First(u => u.UserId == currentPlayer.UserId);

            await Client.GetAsyncWithUser(GameUrl.GetNextTile(game.Id), currentUser, TestContext.Current.CancellationToken);

            await Client.PostAsyncWithUser(GameUrl.PlaceTile(game.Id), currentUser, placeTileDto, TestContext.Current.CancellationToken);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(game.Id), currentUser, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }
}