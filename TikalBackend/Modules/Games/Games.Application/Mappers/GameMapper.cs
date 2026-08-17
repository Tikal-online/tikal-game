using Games.Contracts.Models;
using Games.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Games.Application.Mappers;

[Mapper]
internal static partial class GameMapper
{
    [MapperIgnoreTarget(nameof(GameModel.Players))]
    public static partial GameModel GameToGameModel(Game game);
}