using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

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
        Assert.Multiple(
            () => Assert.NotNull(lobby),
            () => Assert.Equal(createLobbyDto.Name, lobby?.Name),
            () => Assert.Equal(createLobbyDto.MaxPlayers, lobby?.MaxPlayers),
            () => Assert.False(lobby?.InGame),
            () => Assert.Equal(1, lobby?.Players.Count),
            () => Assert.Equal(TestUser.Default.UserId, lobby?.Players.First().UserId),
            () => Assert.Equal(TestUser.Default.Name, lobby?.Players.First().Name)

        );
    }
}