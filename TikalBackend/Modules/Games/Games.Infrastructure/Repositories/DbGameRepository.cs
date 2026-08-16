using Games.Application.DataAccess;
using Games.Domain.Entities;
using Games.Infrastructure.Database;

namespace Games.Infrastructure.Repositories;

internal sealed class DbGameRepository : GameRepository
{
    private readonly GamesDbContext gamesDbContext;

    public DbGameRepository(GamesDbContext gamesDbContext)
    {
        this.gamesDbContext = gamesDbContext;
    }

    public void Create(Game game)
    {
        gamesDbContext.Add(game);
    }
}