using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class CreateLobbyTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenUnauthenticatedUser_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsJsonAsync(LobbyUrl.CreateLobby, createLobbyDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(InvalidCreateLobbyDtos))]
    public Task GivenInvalidCreateLobbyDto_WhenCreateLobby_ThenReturnsBadRequest(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenUserWithoutAccount_WhenCreateLobby_ThenReturnsUnauthorized(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenUserNotInLobby_WhenCreateLobby_ThenReturnsCreated(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyDtos))]
    public Task GivenUserAlreadyInALobby_WhenCreateLobby_ThenReturnsConflict(CreateLobbyDto createLobbyDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);
            await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

            // when
            var response = await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, TestUser.Default, createLobbyDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        });
    }
}