using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class JoinLobbyTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenJoinLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsync(LobbyUrl.JoinLobby(1), null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenJoinLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(1), TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenNoLobbyWithId_WhenJoinLobby_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(1), TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenPlayerAlreadyInLobby_WhenJoinLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

        await CreateUserAccount(TestUser.TestUser1);
        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenFullLobby_WhenJoinLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await CreateUserAccount(TestUser.TestUser1);
        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

        List<TestUser> users = [TestUser.TestUser2, TestUser.TestUser3, TestUser.TestUser4];

        // players join until the lobby is full
        for (var i = 0; i < lobby.MaxPlayers - 1; i++)
        {
            await CreateUserAccount(users[i]);
            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), users[i], null, TestContext.Current.CancellationToken);
        }

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenLobbyInGame_WhenJoinLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await CreateUserAccount(TestUser.TestUser1);
        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

        await CreateUserAccount(TestUser.TestUser2);
        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser2, null, TestContext.Current.CancellationToken);

        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);
        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser2, null, TestContext.Current.CancellationToken);

        await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}