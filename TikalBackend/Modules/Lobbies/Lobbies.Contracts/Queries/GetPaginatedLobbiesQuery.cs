using Lobbies.Contracts.Models;
using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Queries;

public sealed record GetPaginatedLobbiesQuery(int PageSize, int PageNumber, string? SearchText)
    : Query<List<LobbySummaryModel>>;