using System.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRApi.Hubs.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class GlobalChatTests : IntegrationTestFixture
{
    private const string globalChatUrl = "hub/globalChat";

    [Test]
    public void GivenUnauthenticatedUser_WhenConnect_ThenReturnsUnauthorized()
    {
        // when & then
        var exception = Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await CreateConnection(globalChatUrl);
        });

        Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(ChatMessagesTestCases), nameof(ChatMessagesTestCases.ValidChatMessages))]
    public void GivenUserWithoutAccount_WhenSendMessage_ThenThrowsAccountRequiredHubException(string message)
    {
        // when & then
        var exception = Assert.ThrowsAsync<HubException>(async () =>
        {
            await using var connection = await CreateConnection(globalChatUrl, TestUser.Default);
            await connection.InvokeAsync("SendMessage", message);
        });

        Assert.That(exception.Message, Does.Contain("Account required"));
    }

    [TestCaseSource(typeof(ChatMessagesTestCases), nameof(ChatMessagesTestCases.ValidChatMessages))]
    public async Task GivenMultipleConnections_WhenSendMessage_SendsMessageToAllConnections(string message)
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await using var defaultUserConnection = await CreateConnection(globalChatUrl, TestUser.Default);
        var defaultUserReceivedMessageTaskSource = new TaskCompletionSource<ChatMessageDto>();
        defaultUserConnection.On<ChatMessageDto>("ReceiveMessage", defaultUserReceivedMessageTaskSource.SetResult);

        await CreateUserAccount(TestUser.TestUser1);
        await using var user1Connection = await CreateConnection(globalChatUrl, TestUser.TestUser1);
        var user1ReceivedMessageTaskSource = new TaskCompletionSource<ChatMessageDto>();
        user1Connection.On<ChatMessageDto>("ReceiveMessage", user1ReceivedMessageTaskSource.SetResult);

        await CreateUserAccount(TestUser.TestUser2);
        await using var user2Connection = await CreateConnection(globalChatUrl, TestUser.TestUser2);
        var user2ReceivedMessageTaskSource = new TaskCompletionSource<ChatMessageDto>();
        user2Connection.On<ChatMessageDto>("ReceiveMessage", user2ReceivedMessageTaskSource.SetResult);

        // when
        await defaultUserConnection.InvokeAsync("SendMessage", message);

        // then
        TaskCompletionSource<ChatMessageDto>[] taskSources =
        [
            defaultUserReceivedMessageTaskSource,
            user1ReceivedMessageTaskSource,
            user2ReceivedMessageTaskSource
        ];

        using (Assert.EnterMultipleScope())
        {
            foreach (var taskSource in taskSources)
            {
                var receivedMessage = await taskSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

                Assert.That(receivedMessage.Content, Is.EqualTo(message));
                Assert.That(receivedMessage.Username, Is.EqualTo(TestUser.Default.Name));
                Assert.That(receivedMessage.UserId, Is.EqualTo(TestUser.Default.UserId));
            }
        }
    }
}