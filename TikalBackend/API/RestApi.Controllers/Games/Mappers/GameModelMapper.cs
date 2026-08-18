using Games.Contracts.Models;
using RestApi.Controllers.Games.Dtos;
using Riok.Mapperly.Abstractions;

namespace RestApi.Controllers.Games.Mappers;

[Mapper]
internal static partial class GameModelMapper
{
    public static partial GameDto GameModelToGameDto(GameModel gameModel);

    [MapDerivedType<EmptyTileModel, EmptyTileDto>]
    [MapDerivedType<TreasureTileModel, TreasureTileDto>]
    [MapDerivedType<VolcanoTileModel, VolcanoTileDto>]
    [MapDerivedType<TempleTileModel, TempleTileDto>]
    private static partial TileDto TileModelToTileDto(TileModel tile);
}