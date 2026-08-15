using Games.Domain.Entities;

namespace Games.Application.DataAccess;

public interface GameRepository
{
    void Create(Game game);

    Task<Game?> GetByUserId(string userId);
}