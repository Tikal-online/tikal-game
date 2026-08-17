using Games.Application.DataAccess;
using Games.Domain.Entities;
using Games.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.QueryContexts;

internal sealed class DbGameQueryContext : GameQueryContext
{
    private readonly GamesDbContext gamesDbContext;

    public DbGameQueryContext(GamesDbContext gamesDbContext)
    {
        this.gamesDbContext = gamesDbContext;
    }

    public Task<Game?> GetByUserId(string userId)
    {
        return gamesDbContext.Games.AsNoTracking()
            .Include(game => game.Players)
            .Include(game => game.Tiles)
            .ThenInclude(tile => tile.TroopAssignments)
            .AsSplitQuery()
            .SingleOrDefaultAsync(g => g.Players.Any(p => p.UserId == userId));
    }
}