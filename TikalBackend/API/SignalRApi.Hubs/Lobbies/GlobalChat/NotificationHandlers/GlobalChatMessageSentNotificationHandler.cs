using Lobbies.Contracts.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRApi.Hubs.Lobbies.Mappers;

namespace SignalRApi.Hubs.Lobbies.GlobalChat.NotificationHandlers;

internal sealed class GlobalChatMessageSentNotificationHandler : INotificationHandler<GlobalChatMessageSentNotification>
{
    private readonly IHubContext<GlobalChatHub, GlobalChatClient> hubContext;

    public GlobalChatMessageSentNotificationHandler(IHubContext<GlobalChatHub, GlobalChatClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(GlobalChatMessageSentNotification notification, CancellationToken cancellationToken)
    {
        var messageDto = ChatMessageModelMapper.ChatMessageModelToChatMessageDto(notification.message);

        return hubContext.Clients.All.ReceiveMessage(messageDto);
    }
}