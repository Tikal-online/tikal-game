using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Accounts.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts;
using TikalBackend.IntegrationTests.Modules.Lobbies;
using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

internal abstract class IntegrationTestFixture : TestContainerFixture
{
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
        return Client.PostAsyncWithUser(AccountUrl.CreateAccount, user, new CreateAccountDto { Name = user.Name });
    }

    protected async Task<LobbyDto> CreateAndGetLobby(CreateLobbyDto createLobbyDto, TestUser user)
    {
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, user, createLobbyDto);
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, user);
        return (await response.Content.ReadFromJsonAsync<LobbyDto>())!;
    }

    protected async Task<HubConnection> CreateConnection(string url, TestUser? user = null, bool startConnection = true)
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

        if (startConnection)
        {
            await connection.StartAsync();
        }

        return connection;
    }
}