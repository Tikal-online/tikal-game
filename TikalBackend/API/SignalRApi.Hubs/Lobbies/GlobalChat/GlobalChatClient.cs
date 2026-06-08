namespace SignalRApi.Hubs.Lobbies.GlobalChat;

public interface GlobalChatClient
{
    Task ReceiveMessage(string message);
}