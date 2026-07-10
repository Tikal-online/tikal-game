using Lobbies.Contracts.Models;
using Riok.Mapperly.Abstractions;
using SignalRApi.Hubs.Lobbies.Dtos;

namespace SignalRApi.Hubs.Lobbies.Mappers;

[Mapper]
internal static partial class LobbyPlayerModelMapper
{
    public static partial LobbyPlayerDto LobbyPlayerModelToLobbyPlayerDto(LobbyPlayerModel lobbyPlayerModel);
}