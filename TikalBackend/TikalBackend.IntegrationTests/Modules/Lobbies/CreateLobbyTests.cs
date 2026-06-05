using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class CreateLobbyTests : IntegrationTestFixture
{
    private const string createLobbyUrl = "Lobbies";

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUnauthenticatedUser_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        // when
        var response = await Client.PostAsJsonAsync(createLobbyUrl, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.InvalidCreateLobbyDtos))]
    public async Task GivenInvalidCreateLobbyDto_WhenCreateLobby_ThenReturnsBadRequest(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(createLobbyUrl, TestUser.Default, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserWithoutAccount_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        // when
        var response = await Client.PostAsyncWithUser(createLobbyUrl, TestUser.Default, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserNotInLobby_WhenCreateLobby_ThenReturnsCreatedLobby(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(createLobbyUrl, TestUser.Default, createLobbyDto);

        var lobby = await response.Content.ReadFromJsonAsync<LobbyDto>();

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            Assert.That(lobby, Is.Not.Null);
            Assert.That(lobby!.Name, Is.EqualTo(createLobbyDto.Name));
            Assert.That(lobby.MaxPlayers, Is.EqualTo(createLobbyDto.MaxPlayers));


            Assert.That(lobby.Players.Count, Is.EqualTo(1));
            Assert.That(lobby.Players.First().Name, Is.EqualTo(TestUser.Default.Name));
            Assert.That(lobby.Players.First().UserId, Is.EqualTo(TestUser.Default.UserId));
            Assert.That(lobby.Players.First().IsOwner, Is.True);
            Assert.That(lobby.Players.First().IsReady, Is.False);
        }
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserAlreadyInALobby_WhenCreateLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(createLobbyUrl, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.PostAsyncWithUser(createLobbyUrl, TestUser.Default, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }
}