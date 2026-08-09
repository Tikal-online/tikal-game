using Lobbies.Domain.Entities;

namespace Lobbies.Application.DataAccess;

public interface PlayerRepository
{
    void Delete(Player player);

    Task<Player?> GetByUserIdAsync(string userId);
}