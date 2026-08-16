using Games.Domain.Entities;

namespace Games.Application.DataAccess;

public interface GameQueryContext
{
    Task<Game?> GetByUserId(string userId);
}