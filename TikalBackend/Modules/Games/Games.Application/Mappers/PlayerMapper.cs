using Games.Domain.Entities;
using Shared.Contracts.Enums;
using Shared.Domain.Enums;

namespace Games.Application.Mappers;

internal static class PlayerMapper
{
    public static List<Player> DictionaryToPlayers(IReadOnlyDictionary<ColourModel, string> dictionary)
    {
        return
        [
            .. dictionary
                .Select(x => new Player
                {
                    UserId = x.Value,
                    Colour = (Colour)x.Key,
                    Points = 0
                })
        ];
    }
}