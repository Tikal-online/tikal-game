using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class LeaveLobbyTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsync(LobbyUrl.LeaveLobby(1), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(1), TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenLobbyDoesntExists_WhenLeaveLobby_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(1), TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotInLobby_WhenLeaveLobby_ThenReturnsNotFound(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerInLobby_WhenLeaveLobby_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenLobbyInGame_WhenLeaveLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

        await CreateUserAccount(TestUser.TestUser1);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);
        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);

        await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.LeaveLobby(lobby.Id), TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}