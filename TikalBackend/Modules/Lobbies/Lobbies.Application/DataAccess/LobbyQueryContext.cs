using Lobbies.Domain.Entities;

namespace Lobbies.Application.DataAccess;

public interface LobbyQueryContext
{
    Task<Lobby?> GetByIdAsync(long Id);

    Task<Lobby?> GetByUserIdAsync(string userId);

    Task<List<Lobby>> GetPaginatedAsync(int pageSize, int pageNumber, string? searchText);

    Task<int> GetCountAsync(string? searchText);
}