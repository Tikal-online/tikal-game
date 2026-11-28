using System.Net;
using TikalBackend.IntegrationTests.Extensions;

namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class EndTurnTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenEndTurn_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsync(GameUrl.EndTurn(1), null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenUserWithoutAccount_WhenEndTurn_ThenReturnsUnauthorized()
    {
        return RunTracked(async () =>
        {
            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(1), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });
    }

    [Fact]
    public Task GivenNonExistantGame_WhenEndTurn_ThenReturnsNotFound()
    {
        return RunTracked(async () =>
        {
            // given
            await CreateUserAccount(TestUser.Default);

            // when
            var response = await Client.PostAsyncWithUser(GameUrl.EndTurn(1), TestUser.Default, null, TestContext.Current.CancellationToken);

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }
}