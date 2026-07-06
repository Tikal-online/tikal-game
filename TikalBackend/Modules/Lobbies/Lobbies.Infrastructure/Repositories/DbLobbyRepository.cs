using Lobbies.Application.DataAccess;
using Lobbies.Domain.Entities;
using Lobbies.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lobbies.Infrastructure.Repositories;

internal sealed class DbLobbyRepository : LobbyRepository
{
    private readonly LobbiesDbContext lobbiesDbContext;

    public DbLobbyRepository(LobbiesDbContext lobbiesDbContext)
    {
        this.lobbiesDbContext = lobbiesDbContext;
    }

    public void Create(Lobby lobby)
    {
        lobbiesDbContext.Add(lobby);
    }

    public void Delete(Lobby lobby)
    {
        lobbiesDbContext.Remove(lobby);
    }

    public Task<Lobby?> GetByIdAsync(long id)
    {
        return lobbiesDbContext.Lobbies
            .Include(l => l.Players)
            .FirstOrDefaultAsync(l => l.Id == id);
    }
}