using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Queries;
using Shared.Contracts.Queries;

namespace Lobbies.Application.UseCases.GetPaginatedLobbies;

public sealed class GetPaginatedLobbiesQueryHandler
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
        var lobbies = await lobbyQueryContext.GetPaginated(
            request.PageSize,
            request.PageNumber,
            request.SearchText
        );

        var lobbySummaryModels = LobbyMapper.LobbiesToLobbySummaryModels(lobbies);

        var lobbyCount = await lobbyQueryContext.GetCount(request.SearchText);

        return new PaginatedResult<List<LobbySummaryModel>>
        {
            Data = lobbySummaryModels,
            TotalCount = lobbyCount
        };
    }
}