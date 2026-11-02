using Games.Domain.Entities;

namespace Games.Application.DataAccess;

public interface PlayerRepository
{
    void Delete(Player player);
}