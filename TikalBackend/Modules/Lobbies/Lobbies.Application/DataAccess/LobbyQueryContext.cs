using Lobbies.Domain.Entities;

namespace Lobbies.Application.DataAccess;

public interface LobbyQueryContext
{
    Task<Lobby?> GetByIdAsync(long Id);
}