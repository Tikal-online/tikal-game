namespace TikalBackend.IntegrationTests.Modules.Games;

public sealed class ActiveGameTests(AppFactoryFixture fixture) : IntegrationTestFixture(fixture)
{
    [Fact]
    public Task GivenUnauthenticatedUser_WhenConnect_ThenThrowsInvalidOperation()
    {
        // when & then
        return Assert.ThrowsAsync<InvalidOperationException>(() =>
        {
            return CreateConnection(GameUrl.ActiveGameHub);
        });
    }
}