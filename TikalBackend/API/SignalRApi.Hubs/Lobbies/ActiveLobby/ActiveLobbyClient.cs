using SignalRApi.Hubs.Lobbies.Dtos;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby;

public interface ActiveLobbyClient
{
    Task PlayerJoined(LobbyPlayerDto lobbyPlayerDto);
}