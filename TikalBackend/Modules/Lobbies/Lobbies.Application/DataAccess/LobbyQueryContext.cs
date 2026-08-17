using Lobbies.Domain.Entities;

namespace Lobbies.Application.DataAccess;

public interface LobbyQueryContext
{
    Task<Lobby?> GetById(long Id);

    Task<Lobby?> GetByUserId(string userId);

    Task<long?> GetIdByUserId(string userId);

    Task<List<Lobby>> GetPaginated(int pageSize, int pageNumber, string? searchText);

    Task<int> GetCount(string? searchText);
}