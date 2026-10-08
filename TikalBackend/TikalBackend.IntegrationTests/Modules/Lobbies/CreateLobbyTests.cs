using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;
using Xunit;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class CreateLobbyTests : IntegrationTestFixture
{
    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenUnauthenticatedUser_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        // when
        var response = await Client.PostAsJsonAsync(LobbyUrl.CreateLobby, createLobbyDto, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(InvalidCreateLobbyDtos))]
    public async Task GivenInvalidCreateLobbyDto_WhenCreateLobby_ThenReturnsBadRequest(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenUserWithoutAccount_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenUserNotInLobby_WhenCreateLobby_ThenReturnsCreated(
        CreateLobbyDto createLobbyDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public async Task GivenUserAlreadyInALobby_WhenCreateLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto);

        // then
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}