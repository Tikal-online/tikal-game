using System.Net.Http.Json;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using RestApi.Controllers.Accounts.Dtos;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts;
using TikalBackend.IntegrationTests.Modules.Lobbies;
using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

public abstract class IntegrationTestFixture : IAsyncLifetime
{
    protected HttpClient Client;

    public async ValueTask InitializeAsync()
    {
        await CustomWebApplicationFactory.RebuildDatabase(AppFactory.Instance.Services);

        Client = AppFactory.Instance.CreateDefaultClient();
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }

    protected Task CreateUserAccount(TestUser user)
    {
        return Client.PostAsyncWithUser(AccountUrl.CreateAccount, user, new CreateAccountDto { Name = user.Name }, TestContext.Current.CancellationToken);
    }

    protected async Task<LobbyDto> CreateAndGetLobby(CreateLobbyDto createLobbyDto, TestUser user)
    {
        await Client.PostAsyncWithUser(LobbyUrl.CreateLobby, user, createLobbyDto, TestContext.Current.CancellationToken);
        var response = await Client.GetAsyncWithUser(LobbyUrl.GetActiveLobby, user, TestContext.Current.CancellationToken);
        return (await response.Content.ReadFromJsonAsync<LobbyDto>())!;
    }

    protected async Task<HubConnection> CreateConnection(string url, TestUser? user = null, bool startConnection = true)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost/" + url,
                options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                    options.SkipNegotiation = true;

                    options.WebSocketFactory = (context, cancellationToken) =>
                    {
                        var webSocketClient = AppFactory.Instance.Server.CreateWebSocketClient();

                        webSocketClient.ConfigureRequest = request =>
                        {
                            if (user is not null)
                            {
                                request.Headers.Append("X-Test-UserId", user.UserId);
                            }
                        };

                        return new ValueTask<WebSocket>(
                            webSocketClient.ConnectAsync(
                                context.Uri,
                                cancellationToken));
                    };
                })
            .Build();

        if (startConnection)
        {
            var initializationCompleteSource = new TaskCompletionSource();
            connection.On("InitializationComplete", initializationCompleteSource.SetResult);

            await connection.StartAsync(TestContext.Current.CancellationToken);

            // Wait until OnConnectedAsync has completed and the client is assigned to all needed Groups
            await initializationCompleteSource.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        }

        return connection;
    }
}