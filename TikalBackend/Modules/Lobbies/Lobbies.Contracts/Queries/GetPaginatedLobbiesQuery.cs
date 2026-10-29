namespace Lobbies.Contracts.Queries;

public sealed record GetPaginatedLobbiesQuery(int PageSize, int PageNumber, string? SearchText);