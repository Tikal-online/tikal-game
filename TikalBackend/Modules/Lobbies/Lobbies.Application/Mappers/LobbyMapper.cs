using Lobbies.Contracts.Models;
using Lobbies.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Lobbies.Application.Mappers;

[Mapper]
internal static partial class LobbyMapper
{
    [MapperIgnoreTarget(nameof(LobbyModel.Players))]
    public static partial LobbyModel LobbyToLobbyModel(Lobby lobby);

    public static partial List<LobbySummaryModel> LobbiesToLobbySummaryModels(List<Lobby> lobbies);

    [MapProperty(nameof(Lobby.Players), nameof(LobbySummaryModel.CurrentPlayers), Use = nameof(PlayerCount))]
    private static partial LobbySummaryModel LobbyToLobbySummaryModel(Lobby lobby);

    private static int PlayerCount(ICollection<Player> players)
    {
        return players.Count;
    }
}