using Games.Application.DataAccess;
using Games.Domain.Entities;
using Games.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories;

public sealed class DbGameRepository : GameRepository
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

    public void Delete(Game game)
    {
        gamesDbContext.Remove(game);
    }

    public Task<Game?> GetById(long id)
    {
        return gamesDbContext.Games
            .Include(game => game.Players)
            .Include(game => game.Tiles)
            .ThenInclude(tile => tile.TroopAssignments)
            .AsSplitQuery()
            .SingleOrDefaultAsync(g => g.Id == id);
    }
}