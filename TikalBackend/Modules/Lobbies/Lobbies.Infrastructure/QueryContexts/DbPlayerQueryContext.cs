using Lobbies.Application.DataAccess;
using Lobbies.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lobbies.Infrastructure.QueryContexts;

public sealed class DbPlayerQueryContext : PlayerQueryContext
{
    private readonly LobbiesDbContext lobbiesDbContext;

    public DbPlayerQueryContext(LobbiesDbContext lobbiesDbContext)
    {
        this.lobbiesDbContext = lobbiesDbContext;
    }

    public Task<bool> PlayerExists(string UserId, CancellationToken cancellationToken)
    {
        return lobbiesDbContext.Players.AsNoTracking().AnyAsync(x => x.UserId == UserId, cancellationToken);
    }
}