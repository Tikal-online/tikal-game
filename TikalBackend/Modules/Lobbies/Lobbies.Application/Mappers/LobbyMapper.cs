using Lobbies.Contracts.Models;
using Lobbies.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Lobbies.Application.Mappers;

[Mapper]
internal static partial class LobbyMapper
{
    [MapperIgnoreTarget(nameof(LobbyModel.Players))]
    public static partial LobbyModel LobbyToLobbyModel(Lobby lobby);
}