using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Games.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class GetGameForAuthenticatedPlayerTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGetGameForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsync(GameUrl.GetActiveGame, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenGetGameForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsyncWithUser(GameUrl.GetActiveGame, TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenPlayerNotInAGame_WhenGetGameForAuthenticatedPlayer_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.GetAsyncWithUser(GameUrl.GetActiveGame, TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerInAGame_WhenGetGameForAuthenticatedPlayer_ThenReturnsGame(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null);

        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null);
        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null);

        await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null);

        // when
        var response = await Client.GetAsyncWithUser(GameUrl.GetActiveGame, TestUser.Default);

        var game = await response.Content.ReadFromJsonAsync<GameDto>(TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(game);

        TestUser[] expectedPlayers = [TestUser.Default, TestUser.TestUser1];

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Equal(expectedPlayers.Length, game.Players.Count),
            () => Assert.Equal(4, game.Tiles.Count),
            () =>
            {
                foreach (var expectedPlayer in expectedPlayers)
                {
                    var player = game.Players.FirstOrDefault(p => p.UserId == expectedPlayer.UserId);

                    Assert.Multiple(
                        () => Assert.NotNull(player),
                        () => Assert.Equal(expectedPlayer.UserId, player?.UserId),
                        () => Assert.Equal(expectedPlayer.Name, player?.Name),
                        () => Assert.Equal(0, player?.Points)
                    );
                }
            }
        );
    }
}