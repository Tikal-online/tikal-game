using Games.Domain.Entities;

namespace Games.Application.DataAccess;

public interface GameRepository
{
    void Create(Game game);

    void Delete(Game game);

    Task<Game?> GetById(long id);
}