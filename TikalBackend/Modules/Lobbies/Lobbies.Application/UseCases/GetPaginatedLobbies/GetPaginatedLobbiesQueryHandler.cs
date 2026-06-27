using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Queries;
using Shared.Contracts.Messaging;
using Shared.Contracts.Queries;

namespace Lobbies.Application.UseCases.GetPaginatedLobbies;

internal sealed class GetPaginatedLobbiesQueryHandler
    : QueryHandler<GetPaginatedLobbiesQuery, PaginatedResult<List<LobbySummaryModel>>>
{
    private readonly LobbyQueryContext lobbyQueryContext;

    public GetPaginatedLobbiesQueryHandler(LobbyQueryContext lobbyQueryContext)
    {
        this.lobbyQueryContext = lobbyQueryContext;
    }

    public async Task<PaginatedResult<List<LobbySummaryModel>>> Handle(
        GetPaginatedLobbiesQuery request,
        CancellationToken cancellationToken
    )
    {
        var lobbies = await lobbyQueryContext.GetPaginatedAsync(
            request.PageSize,
            request.PageNumber,
            request.SearchText
        );

        var lobbySummaryModels = LobbyMapper.LobbiesToLobbySummaryModels(lobbies);

        var lobbyCount = await lobbyQueryContext.GetCountAsync();

        return new PaginatedResult<List<LobbySummaryModel>>
        {
            Data = lobbySummaryModels,
            TotalCount = lobbyCount
        };
    }
}