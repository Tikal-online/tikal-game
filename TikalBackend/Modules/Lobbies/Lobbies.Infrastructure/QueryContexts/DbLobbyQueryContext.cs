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

    public Task<Lobby?> GetById(long Id)
    {
        return lobbiesDbContext.Lobbies.AsNoTracking()
            .Include(l => l.Players)
            .FirstOrDefaultAsync(l => l.Id == Id);
    }

    public Task<Lobby?> GetByUserId(string userId)
    {
        return lobbiesDbContext.Lobbies.AsNoTracking()
            .Include(l => l.Players)
            .Where(l => l.Players.Any(p => p.UserId == userId))
            .FirstOrDefaultAsync();
    }

    public Task<long?> GetIdByUserId(string userId)
    {
        return lobbiesDbContext.Lobbies.AsNoTracking()
            .Where(l => l.Players.Any(p => p.UserId == userId))
            .Select(l => (long?)l.Id)
            .FirstOrDefaultAsync();
    }

    public Task<List<Lobby>> GetPaginated(int pageSize, int pageNumber, string? searchText)
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

    public Task<int> GetCount(string? searchText)
    {
        var query = lobbiesDbContext.Lobbies.AsNoTracking();

        if (searchText is not null)
        {
            query = query.Where(l => l.Name.ToLower().Contains(searchText.ToLower()));
        }

        return query.CountAsync();
    }
}