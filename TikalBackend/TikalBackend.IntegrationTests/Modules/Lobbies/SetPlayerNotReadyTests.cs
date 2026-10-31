using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class SetPlayerNotReadyTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenSetPlayerNotReady_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.DeleteAsync(LobbyUrl.SetPlayerNotReady, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenUserWithoutAccount_WhenSetPlayerNotReady_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenUserNotInALobby_WhenSetPlayerNotReady_ThenReturnsNotFound()
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenUserInALobby_WhenSetPlayerNotReady_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

            // when
            var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }
}