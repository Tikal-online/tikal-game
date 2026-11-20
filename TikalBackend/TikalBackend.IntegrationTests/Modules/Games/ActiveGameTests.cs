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

    [Fact]
    public async Task GivenuserWithoutAccount_WhenConnect_ThenThrowsAccountRequiredHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.Default, false);
        connection.Closed += ex =>
        {
            closedExceptionSource.TrySetResult(ex);
            return Task.CompletedTask;
        };

        // when
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // then
        var exception = await closedExceptionSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.NotNull(exception);
        Assert.Contains("Account required", exception.Message);
    }

    [Fact]
    public async Task GivenUserNotInAGame_WhenConnect_ThenThrowsNotInAGameHubException()
    {
        // given
        var closedExceptionSource = new TaskCompletionSource<Exception?>();
        await CreateUserAccount(TestUser.Default);
        await using var connection = await CreateConnection(GameUrl.ActiveGameHub, TestUser.Default, false);
        connection.Closed += ex =>
        {
            closedExceptionSource.TrySetResult(ex);
            return Task.CompletedTask;
        };

        // when
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // then
        var exception = await closedExceptionSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.NotNull(exception);
        Assert.Contains("Player is not in a game", exception.Message);

    }
}