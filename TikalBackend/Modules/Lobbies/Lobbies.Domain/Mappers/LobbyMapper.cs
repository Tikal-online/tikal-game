using Lobbies.Domain.Entities;
using Lobbies.Domain.EventData;
using Riok.Mapperly.Abstractions;

namespace Lobbies.Domain.Mappers;

[Mapper]
internal static partial class LobbyMapper
{
    public static partial LobbyEventData LobbyToLobbyEventData(Lobby lobby);
}