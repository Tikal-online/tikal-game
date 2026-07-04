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

    public Task<Lobby?> GetByUserIdAsync(string userId)
    {
        return lobbiesDbContext.Lobbies.AsNoTracking()
            .Include(l => l.Players)
            .Where(l => l.Players.Any(p => p.UserId == userId))
            .FirstOrDefaultAsync();
    }

    public Task<List<Lobby>> GetPaginatedAsync(int pageSize, int pageNumber, string? searchText)
    {
        IQueryable<Lobby> query = lobbiesDbContext.Lobbies.AsNoTracking()
            .Include(l => l.Players);

        if (searchText is not null)
        {
            query = query.Where(l => l.Name.ToLower().Contains(searchText.ToLower()));
        }

        return query
            .OrderBy(l => l.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<int> GetCountAsync(string? searchText)
    {
        var query = lobbiesDbContext.Lobbies.AsNoTracking();

        if (searchText is not null)
        {
            query = query.Where(l => l.Name.ToLower().Contains(searchText.ToLower()));
        }

        return query.CountAsync();
    }
}