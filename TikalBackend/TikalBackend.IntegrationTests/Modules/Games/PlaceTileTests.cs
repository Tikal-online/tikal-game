using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Games.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Games.Dtos;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class PlaceTileTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Theory]
    [ClassData(typeof(ValidPlaceTileDtos))]
    public Task GivenUnauthenticatedUser_WhenPlaceTile_ThenReturnsUnauthorized(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsJsonAsync(GameUrl.PlaceTile(1), placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidPlaceTileDtos))]
    public Task GivenUserWithoutAccount_WhenPlaceTile_ThenReturnsUnauthorized(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(1), TestUser.Default, placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(InvalidPlaceTileDtos))]
    public Task GivenInvalidPlaceTileDto_WhenPlaceTile_ThenReturnsUnauthorized(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(1), TestUser.Default, placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidPlaceTileDtos))]
    public Task GivenNonExistantGame_WhenPlaceTile_ThenReturnsNotFound(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(1), TestUser.Default, placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotInGame_WhenPlaceTile_ThenReturnsForbidden(CreateLobbyDto createLobbyDto)
    {
        // given
        var dto = new PlaceTileDto() { Q = 1, R = 1, Rotation = 1 };

        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await CreateUserAccount(TestUser.TestUser2);

        await RunTracked(async () =>
        {
            // when
            var game = await GetActiveGame(TestUser.Default);

            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(game.Id), TestUser.TestUser2, dto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotCurrentPlayer_WhenPlaceTile_ThenReturnsForbidden(CreateLobbyDto createLobbyDto)
    {
        // given
        var dto = new PlaceTileDto() { Q = 1, R = 1, Rotation = 1 };

        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            // when
            var game = await GetActiveGame(TestUser.Default);

            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var notCurrentUser = users.First(u => u.UserId != currentPlayer.UserId);

            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(game.Id), notCurrentUser, dto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenNoDrawnTile_WhenPlaceTile_ThenReturnsNotFound(CreateLobbyDto createLobbyDto)
    {
        // given
        var dto = new PlaceTileDto() { Q = 1, R = 1, Rotation = 1 };

        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            // when
            var game = await GetActiveGame(TestUser.Default);

            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var currentUser = users.First(u => u.UserId == currentPlayer.UserId);

            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(game.Id), currentUser, dto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenInvalidTilePosition_WhenPlaceTile_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        var dto = new PlaceTileDto() { Q = 5, R = 5, Rotation = 1 };

        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            // when
            var game = await GetActiveGame(TestUser.Default);

            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var currentUser = users.First(u => u.UserId == currentPlayer.UserId);

            await Client.GetAsyncWithUser(GameUrl.GetNextTile(game.Id), currentUser, TestContext.Current.CancellationToken);

            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(game.Id), currentUser, dto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenValidTilePosition_WhenPlaceTile_ThenReturnsOk(CreateLobbyDto createLobbyDto)
    {
        // given
        var dto = new PlaceTileDto() { Q = 1, R = -2, Rotation = 2 };

        await CreateAndStartLobby([TestUser.Default, TestUser.TestUser1], createLobbyDto);

        await RunTracked(async () =>
        {
            // when
            var game = await GetActiveGame(TestUser.Default);

            var currentPlayer = game.Players.Single(p => p.TurnPosition == game.CurrentTurnPosition);
            List<TestUser> users = [TestUser.Default, TestUser.TestUser1];
            var currentUser = users.First(u => u.UserId == currentPlayer.UserId);

            await Client.GetAsyncWithUser(GameUrl.GetNextTile(game.Id), currentUser, TestContext.Current.CancellationToken);

            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(game.Id), currentUser, dto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }
}