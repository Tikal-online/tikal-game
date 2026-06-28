using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Accounts.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

internal abstract class IntegrationTestFixture : TestContainerFixture
{
    private const string createAccountUrl = "Accounts";

    private CustomWebApplicationFactory factory;

    protected HttpClient Client { get; private set; }

    [SetUp]
    public void Setup()
    {
        factory = new CustomWebApplicationFactory(DatabaseContainer.GetConnectionString());
        Client = factory.CreateDefaultClient();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
        factory.Dispose();
    }

    protected Task CreateUserAccount(TestUser user)
    {
        return Client.PostAsyncWithUser(createAccountUrl, user, new CreateAccountDto { Name = user.Name });
    }

    protected async Task<HubConnection> CreateConnection(string url, TestUser? user = null)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("wss://localhost/" + url,
                options =>
                {
                    options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();

                    if (user is not null)
                    {
                        options.Headers["X-Test-UserId"] = user.UserId;
                    }
                })
            .Build();

        await connection.StartAsync();

        return connection;
    }
}