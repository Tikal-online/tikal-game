using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Queries;
using MediatR;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.GetLobby;

internal sealed class GetLobbyQueryHandler : QueryHandler<GetLobbyQuery, LobbyModel?>
{
    private readonly LobbyQueryContext lobbyQueryContext;

    private readonly ISender sender;

    public GetLobbyQueryHandler(LobbyQueryContext lobbyQueryContext, ISender sender)
    {
        this.lobbyQueryContext = lobbyQueryContext;
        this.sender = sender;
    }

    public async Task<LobbyModel?> Handle(GetLobbyQuery request, CancellationToken cancellationToken)
    {
        var lobby = await lobbyQueryContext.GetByIdAsync(request.Id);

        if (lobby is null)
        {
            return null;
        }

        var userIds = lobby.Players.Select(p => p.UserId).ToHashSet();

        var accounts = await sender.Send(new GetAccountsQuery(userIds), cancellationToken);

        var playerModels = PlayerMapper.PlayersToLobbyPlayerModels(lobby.Players, accounts);

        var lobbyModel = LobbyMapper.LobbyToLobbyModel(lobby);

        lobbyModel.Players = playerModels;

        return lobbyModel;
    }
}