using Games.Domain.Entities;
using Games.Domain.EventData;
using Riok.Mapperly.Abstractions;

[Mapper]
internal static partial class GameMapper
{
    public static partial GameEventData GameToGameEventData(Game game);
}