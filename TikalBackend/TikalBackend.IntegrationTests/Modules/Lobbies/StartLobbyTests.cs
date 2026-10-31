using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class StartLobbyTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenStartLobby_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsync(LobbyUrl.StartLobby(1), null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenUserWithoutAccount_WhenStartLobby_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(1), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenNoLobbyWithId_WhenStartLobby_ThenReturnsNotFound()
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(1), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenPlayerNotInLobby_WhenStartLobby_ThenReturnsNotFound(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            await CreateUserAccount(TestUser.TestUser1);

            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenPlayerIsNotOwner_WhenStartLobby_ThenReturnsForbidden(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            await CreateUserAccount(TestUser.TestUser1);

            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenNotAllPlayersReady_WhenStartLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            await CreateUserAccount(TestUser.TestUser1);

            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenAllPlayersReady_WhenStartLobby_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            await CreateUserAccount(TestUser.TestUser1);

            var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

            await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);
            await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null, TestContext.Current.CancellationToken);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }
}