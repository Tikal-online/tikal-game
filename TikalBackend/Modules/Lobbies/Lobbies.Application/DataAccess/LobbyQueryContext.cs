using Lobbies.Domain.Entities;

namespace Lobbies.Application.DataAccess;

public interface LobbyQueryContext
{
    Task<Lobby?> GetById(long Id, CancellationToken cancellationToken);

    Task<Lobby?> GetByUserId(string userId, CancellationToken cancellationToken);

    Task<long?> GetIdByUserId(string userId, CancellationToken cancellationToken);

    Task<List<Lobby>> GetPaginated(int pageSize, int pageNumber, string? searchText, CancellationToken cancellationToken);

    Task<int> GetCount(string? searchText, CancellationToken cancellationToken);
}