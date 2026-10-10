using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class SetPlayerReadyTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenSetPlayerReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PutAsync(LobbyUrl.SetPlayerReady, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenSetPlayerReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserNotInALobby_WhenSetPlayerReady_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenUserInALobby_WhenSetPlayerReady_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

        // when
        var response = await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}