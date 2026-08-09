using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class SetPlayerNotReadyTests : IntegrationTestFixture
{
    [Test]
    public async Task GivenUnauthenticatedUser_WhenSetPlayerNotReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsync(LobbyUrl.SetPlayerNotReady);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenSetPlayerNotReady_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserNotInALobby_WhenSetPlayerNotReady_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserInALobby_WhenSetPlayerNotReady_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.DeleteAsyncWithUser(LobbyUrl.SetPlayerNotReady, TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}