using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;
using Xunit;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class GetLobbyForAuthenticatedPlayerTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsync(LobbyUrl.GetActiveLobby, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenPlayerNotInALobby_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerInALobby_WhenGetLobbyForAuthenticatedPlayer_ThenReturnsLobby(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, TestUser.Default);

        var lobby = await response.Content.ReadFromJsonAsync<LobbyDto>(TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(lobby);

        // TODO: assertions
        /*
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(lobby.Name, Is.EqualTo(createLobbyDto.Name));
            Assert.That(lobby.MaxPlayers, Is.EqualTo(createLobbyDto.MaxPlayers));
            Assert.That(lobby.InGame, Is.False);

            Assert.That(lobby.Players, Has.Count.EqualTo(1));
            Assert.That(lobby.Players[0].UserId, Is.EqualTo(TestUser.Default.UserId));
            Assert.That(lobby.Players[0].Name, Is.EqualTo(TestUser.Default.Name));
        }
        */
    }
}