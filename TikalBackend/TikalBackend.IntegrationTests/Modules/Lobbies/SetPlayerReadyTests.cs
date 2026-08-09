using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class SetPlayerReadyTests : IntegrationTestFixture
{
    [Test]
    public async Task GivenUnauthenticatedUser_WhenSetPlayerReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PutAsync(LobbyUrl.SetPlayerReady, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenSetPlayerReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserNotInALobby_WhenSetPlayerReady_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserInALobby_WhenSetPlayerReady_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}