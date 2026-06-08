using Lobbies.Contracts.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApi.Hubs.Lobbies.GlobalChat.NotificationHandlers;

internal sealed class GlobalChatMessageSentNotificationHandler : INotificationHandler<GlobalChatMessageSentNotification>
{
    private readonly IHubContext<GlobalChatHub, GlobalChatClient> hubContext;

    public GlobalChatMessageSentNotificationHandler(IHubContext<GlobalChatHub, GlobalChatClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public async Task Handle(GlobalChatMessageSentNotification notification, CancellationToken cancellationToken)
    {
        await hubContext.Clients.All.ReceiveMessage(notification.message);
    }
}