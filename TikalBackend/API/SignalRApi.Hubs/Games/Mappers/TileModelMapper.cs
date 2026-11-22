using Games.Contracts.Models;
using Riok.Mapperly.Abstractions;
using SignalRApi.Hubs.Games.Dtos;

namespace SignalRApi.Hubs.Games.Mappers;

[Mapper]
internal static partial class TileModelMapper
{
    [MapDerivedType<EmptyTileModel, EmptyTileDto>]
    [MapDerivedType<TreasureTileModel, TreasureTileDto>]
    [MapDerivedType<VolcanoTileModel, VolcanoTileDto>]
    [MapDerivedType<TempleTileModel, TempleTileDto>]
    public static partial TileDto TileModelToTileDto(TileModel tile);
}