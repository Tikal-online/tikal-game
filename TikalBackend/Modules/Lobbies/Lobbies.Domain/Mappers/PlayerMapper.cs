using Lobbies.Domain.Entities;
using Lobbies.Domain.EventData;
using Riok.Mapperly.Abstractions;

namespace Lobbies.Domain.Mappers;

[Mapper]
internal static partial class PlayerMapper
{
    public static partial PlayerEventData PlayerToPlayerEventData(Player player);
}