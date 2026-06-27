using Lobbies.Contracts.Models;
using Shared.Contracts.Messaging;
using Shared.Contracts.Queries;

namespace Lobbies.Contracts.Queries;

public sealed record GetPaginatedLobbiesQuery(int PageSize, int PageNumber, string? SearchText)
    : Query<PaginatedResult<List<LobbySummaryModel>>>;