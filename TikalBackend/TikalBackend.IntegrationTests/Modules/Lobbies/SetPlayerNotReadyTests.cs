using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class SetPlayerNotReadyTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenSetPlayerNotReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsync(LobbyUrl.SetPlayerNotReady, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenSetPlayerNotReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserNotInALobby_WhenSetPlayerNotReady_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenUserInALobby_WhenSetPlayerNotReady_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default);

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}