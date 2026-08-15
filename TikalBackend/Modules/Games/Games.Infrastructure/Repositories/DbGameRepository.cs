using Games.Application.DataAccess;
using Games.Domain.Entities;
using Games.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

    public Task<Game?> GetByUserId(string userId)
    {
        return gamesDbContext.Games
            .Include(game => game.Players)
            .Include(game => game.TileMap)
            .ThenInclude(tileMap => tileMap.Tiles)
            .ThenInclude(tile => tile.TroopAssignments)
            .AsSplitQuery()
            .SingleOrDefaultAsync(g => g.Players.Any(p => p.UserId == userId));
    }
}