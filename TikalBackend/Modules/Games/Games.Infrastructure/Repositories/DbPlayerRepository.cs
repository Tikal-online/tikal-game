using Games.Application.DataAccess;
using Games.Domain.Entities;
using Games.Infrastructure.Database;

namespace Games.Infrastructure.Repositories;

public sealed class DbPlayerRepository : PlayerRepository
{
    private readonly GamesDbContext gamesDbContext;

    public DbPlayerRepository(GamesDbContext gamesDbContext)
    {
        this.gamesDbContext = gamesDbContext;
    }

    public void Delete(Player player)
    {
        gamesDbContext.Remove(player);
    }
}