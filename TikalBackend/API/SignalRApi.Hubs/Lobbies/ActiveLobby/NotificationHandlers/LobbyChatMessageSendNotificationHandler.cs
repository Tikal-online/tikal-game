using Lobbies.Contracts.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRApi.Hubs.Lobbies.Mappers;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby.NotificationHandlers;

internal sealed class LobbyChatMessageSendNotificationHandler : INotificationHandler<LobbyChatMessageSentNotification>
{
    private readonly IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext;

    public LobbyChatMessageSendNotificationHandler(IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(LobbyChatMessageSentNotification notification, CancellationToken cancellationToken)
    {
        var chatMessageDto = ChatMessageModelMapper.ChatMessageModelToChatMessageDto(notification.ChatMessageModel);

        return hubContext.Clients.Group($"{notification.LobbyId}").ReceiveMessage(chatMessageDto);
    }
}