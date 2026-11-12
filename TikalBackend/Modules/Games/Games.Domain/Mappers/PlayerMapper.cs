using Games.Domain.Entities;
using Games.Domain.EventData;
using Riok.Mapperly.Abstractions;

namespace Games.Domain.Mappers;

[Mapper]
internal static partial class PlayerMapper
{
    public static partial PlayerEventData PlayerToPlayerEventData(Player player);
}