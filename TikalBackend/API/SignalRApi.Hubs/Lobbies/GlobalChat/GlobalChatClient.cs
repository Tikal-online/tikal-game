using SignalRApi.Hubs.Lobbies.Dtos;

namespace SignalRApi.Hubs.Lobbies.GlobalChat;

public interface GlobalChatClient
{
    Task ReceiveMessage(ChatMessageDto messageDto);
}