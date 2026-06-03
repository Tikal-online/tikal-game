using Lobbies.Application.DataAccess;
using Lobbies.Domain.Entities;
using Lobbies.Infrastructure.Database;

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
}