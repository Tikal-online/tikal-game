using Lobbies.Application.DataAccess;
using Lobbies.Domain.Entities;
using Lobbies.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lobbies.Infrastructure.QueryContexts;

internal sealed class DbLobbyQueryContext : LobbyQueryContext
{
    private readonly LobbiesDbContext lobbiesDbContext;

    public DbLobbyQueryContext(LobbiesDbContext lobbiesDbContext)
    {
        this.lobbiesDbContext = lobbiesDbContext;
    }

    public Task<Lobby?> GetByIdAsync(long Id)
    {
        return lobbiesDbContext.Lobbies.AsNoTracking()
            .Include(l => l.Players)
            .FirstOrDefaultAsync(l => l.Id == Id);
    }
}