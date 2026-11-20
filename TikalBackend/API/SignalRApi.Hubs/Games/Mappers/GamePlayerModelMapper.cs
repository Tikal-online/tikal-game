using Games.Contracts.Models;
using Riok.Mapperly.Abstractions;
using SignalRApi.Hubs.Games.Dtos;

namespace SignalRApi.Hubs.Games.Mappers;

[Mapper]
internal static partial class GamePlayerModelMapper
{
    public static partial GamePlayerDto GamePlayerModelToGamePlayerDto(GamePlayerModel gamePlayerModel);
}