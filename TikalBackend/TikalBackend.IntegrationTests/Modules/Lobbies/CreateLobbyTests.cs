using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class CreateLobbyTests : IntegrationTestFixture
{
    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUnauthenticatedUser_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        // when
        var response = await Client.PostAsJsonAsync(LobbyUrl.CreateLobby, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.InvalidCreateLobbyDtos))]
    public async Task GivenInvalidCreateLobbyDto_WhenCreateLobby_ThenReturnsBadRequest(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserWithoutAccount_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserNotInLobby_WhenCreateLobby_ThenReturnsCreated(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [TestCaseSource(typeof(CreateLobbyDtoTestCases), nameof(CreateLobbyDtoTestCases.ValidCreateLobbyDtos))]
    public async Task GivenUserAlreadyInALobby_WhenCreateLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }
}