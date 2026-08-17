using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Games.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

internal sealed class GetGameForAuthenticatedPlayerTests : IntegrationTestFixture
{
    [Test]
    public async Task GivenUnauthenticatedUser_WhenGetGameForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var result = await Client.GetAsync(GameUrl.GetActiveGame);

        // then
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenGetGameForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var result = await Client.GetAsyncWithUser(GameUrl.GetActiveGame, TestUser.Default);

        // then
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenPlayerNotInAGame_WhenGetGameForAuthenticatedPlayer_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var result = await Client.GetAsyncWithUser(GameUrl.GetActiveGame, TestUser.Default);

        // then
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
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

        var game = await response.Content.ReadFromJsonAsync<GameDto>();

        // then
        Assert.That(game, Is.Not.Null);

        TestUser[] expectedPlayers = [TestUser.Default, TestUser.TestUser1];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(game.Players, Has.Count.EqualTo(expectedPlayers.Length));

            foreach (var expectedPlayer in expectedPlayers)
            {
                var player = game.Players.FirstOrDefault(p => p.UserId == expectedPlayer.UserId);

                Assert.That(player, Is.Not.Null);
                Assert.That(player.UserId, Is.EqualTo(expectedPlayer.UserId));
                Assert.That(player.Name, Is.EqualTo(expectedPlayer.Name));
                Assert.That(player.Points, Is.EqualTo(0));
            }
        }
    }
}