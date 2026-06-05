using Lobbies.Contracts.Models;
using RestApi.Controllers.Lobbies.Dtos;
using Riok.Mapperly.Abstractions;

namespace RestApi.Controllers.Lobbies.Mappers;

[Mapper]
internal static partial class LobbyModelMapper
{
    public static partial LobbyDto LobbyModelToLobbyDto(LobbyModel lobbyModel);

    public static partial List<LobbySummaryDto> LobbySummaryModelsToLobbySummaryDtos(
        List<LobbySummaryModel> lobbySummaryModels
    );
}