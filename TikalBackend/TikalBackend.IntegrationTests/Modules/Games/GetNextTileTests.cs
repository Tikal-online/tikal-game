using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Games.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class GetNextTileTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenGetNextTile_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.GetAsync(GameUrl.GetNextTile(1), TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenUserWithoutAccount_WhenGetNextTile_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.GetAsyncWithUser(GameUrl.GetNextTile(1), TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenGameDoesntExist_WhenGetNextTile_ThenReturnsNotFound()
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.GetAsyncWithUser(GameUrl.GetNextTile(1), TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenGameWithTilesLeft_WhenGetNextTile_ThenReturnsTile(CreateLobbyDto createLobbyDto)
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
            // when
            var game = await GetActiveGame(TestUser.Default);

            var response = await Client.GetAsyncWithUser(GameUrl.GetNextTile(game.Id), TestUser.Default, TestContext.Current.CancellationToken);

            var tile = await response.Content.ReadFromJsonAsync<TileDto>(TestContext.Current.CancellationToken);

            // then
            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.NotNull(tile)
            );
        });
    }
}