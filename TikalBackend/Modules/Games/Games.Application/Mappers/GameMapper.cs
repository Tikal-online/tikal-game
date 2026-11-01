using Games.Contracts.Models;
using Games.Domain.Entities;
using Games.Domain.EventData;
using Riok.Mapperly.Abstractions;

namespace Games.Application.Mappers;

[Mapper]
internal static partial class GameMapper
{
    // Entity --> Model
    public static partial GameModel GameToGameModel(Game game);

    [MapDerivedType<EmptyTile, EmptyTileModel>]
    [MapDerivedType<TreasureTile, TreasureTileModel>]
    [MapDerivedType<VolcanoTile, VolcanoTileModel>]
    [MapDerivedType<TempleTile, TempleTileModel>]
    private static partial TileModel TileToTileModel(Tile tile);

    // EventData --> Model
    public static partial GameModel GameEventDataToGameModel(GameEventData game);

    [MapDerivedType<EmptyTileEventData, EmptyTileModel>]
    [MapDerivedType<TreasureTileEventData, TreasureTileModel>]
    [MapDerivedType<VolcanoTileEventData, VolcanoTileModel>]
    [MapDerivedType<TempleTileEventData, TempleTileModel>]
    private static partial TileModel TileEventDataToTileModel(TileEventData tile);
}