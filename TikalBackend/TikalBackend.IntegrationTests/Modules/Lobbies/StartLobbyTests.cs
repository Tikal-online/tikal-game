using System.Net;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class StartLobbyTests : IntegrationTestFixture
{
    [Test]
    public async Task GivenUnauthenticatedUser_WhenStartLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsync(LobbyUrl.StartLobby(1), null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenStartLobby_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(1), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenNoLobbyWithId_WhenStartLobby_ThenReturnsNotFound()
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(1), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerNotInLobby_WhenStartLobby_ThenReturnsNotFound(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenPlayerIsNotOwner_WhenStartLobby_ThenReturnsForbidden(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.Default, null);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenNotAllPlayersReady_WhenStartLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenAllPlayersReady_WhenStartLobby_ThenReturnsSuccess(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

        await Client.PostAsyncWithUser(LobbyUrl.JoinLobby(lobby.Id), TestUser.TestUser1, null);

        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.Default, null);
        await Client.PutAsyncWithUser(LobbyUrl.SetPlayerReady, TestUser.TestUser1, null);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.StartLobby(lobby.Id), TestUser.Default, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}