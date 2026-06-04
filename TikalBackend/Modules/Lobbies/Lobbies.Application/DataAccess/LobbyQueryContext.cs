using Lobbies.Domain.Entities;

namespace Lobbies.Application.DataAccess;

public interface LobbyQueryContext
{
    Task<Lobby?> GetByIdAsync(long Id);

    Task<List<Lobby>> GetPaginatedAsync(int pageSize, int pageNumber, string? searchText);
}