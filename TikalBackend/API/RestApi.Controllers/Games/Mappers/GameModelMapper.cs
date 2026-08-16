using Games.Contracts.Models;
using RestApi.Controllers.Games.Dtos;
using Riok.Mapperly.Abstractions;

namespace RestApi.Controllers.Games.Mappers;

[Mapper]
internal static partial class GameModelMapper
{
    public static partial GameDto GameModelToGameDto(GameModel gameModel);
}