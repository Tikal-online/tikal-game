using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class LeaveLobbyTests : IntegrationTestFixture
{
    private const string leaveLobbyUrl = "Lobbies/leave";

    private const string createLobbyUrl = "Lobbies";

    [Test]
    public async Task GivenUnauthenticatedUser_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsync(leaveLobbyUrl, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenLeaveLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsyncWithUser(leaveLobbyUrl, TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenPlayerNotInLobby_WhenLeaveLobby_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(leaveLobbyUrl, TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerInLobby_WhenLeaveLobby_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        await Client.PostAsyncWithUser(createLobbyUrl, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.PostAsyncWithUser(leaveLobbyUrl, TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}