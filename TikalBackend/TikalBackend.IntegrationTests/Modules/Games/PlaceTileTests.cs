using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Games.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Games.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class PlaceTileTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Theory]
    [ClassData(typeof(ValidPlaceTileDtos))]
    public Task GivenUnauthenticatedUser_WhenPlaceTile_ThenReturnsUnauthorized(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsJsonAsync(GameUrl.PlaceTile(1), placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidPlaceTileDtos))]
    public Task GivenUserWithoutAccount_WhenPlaceTile_ThenReturnsUnauthorized(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(1), TestUser.Default, placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(InvalidPlaceTileDtos))]
    public Task GivenInvalidPlaceTileDto_WhenPlaceTile_ThenReturnsUnauthorized(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(1), TestUser.Default, placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Theory]
    [ClassData(typeof(ValidPlaceTileDtos))]
    public Task GivenNonExistantGame_WhenPlaceTile_ThenReturnsNotFound(PlaceTileDto placeTileDto)
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.PlaceTile(1), TestUser.Default, placeTileDto, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }
}