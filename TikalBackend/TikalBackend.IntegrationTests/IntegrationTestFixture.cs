using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Accounts.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts;
using TikalBackend.IntegrationTests.Modules.Lobbies;
using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

public abstract class IntegrationTestFixture : IDisposable
{
    private readonly CustomWebApplicationFactory factory;

    protected readonly HttpClient Client;

    public IntegrationTestFixture()
    {
        factory = new CustomWebApplicationFactory(PostgresDatabase.Instance.GetConnectionString());
        Client = factory.CreateDefaultClient();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Client.Dispose();
            factory.Dispose();
        }
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
            var initializationCompleteSource = new TaskCompletionSource();
            connection.On("InitializationComplete", initializationCompleteSource.SetResult);

            await connection.StartAsync();

            // Wait until OnConnectedAsync has completed and the client is assigned to all needed Groups
            await initializationCompleteSource.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }

        return connection;
    }
}