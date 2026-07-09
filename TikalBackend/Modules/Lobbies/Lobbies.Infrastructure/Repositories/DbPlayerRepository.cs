using Lobbies.Application.DataAccess;
using Lobbies.Domain.Entities;
using Lobbies.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lobbies.Infrastructure.Repositories;

internal sealed class DbPlayerRepository : PlayerRepository
{
    private readonly LobbiesDbContext lobbiesDbContext;

    public DbPlayerRepository(LobbiesDbContext lobbiesDbContext)
    {
        this.lobbiesDbContext = lobbiesDbContext;
    }

    public void Delete(Player player)
    {
        lobbiesDbContext.Remove(player);
    }

    public Task<Player?> GetByUserIdWithLobbyAsync(string userId)
    {
        return lobbiesDbContext.Players
            .Where(p => p.UserId == userId)
            .Include(p => p.Lobby)
            .ThenInclude(l => l.Players)
            .FirstOrDefaultAsync();
    }
}