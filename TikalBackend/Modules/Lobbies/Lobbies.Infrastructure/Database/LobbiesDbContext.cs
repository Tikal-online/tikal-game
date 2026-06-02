using Lobbies.Application.DataAccess;
using Lobbies.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lobbies.Infrastructure.Database;

public sealed class LobbiesDbContext : DbContext, UnitOfWork
{
    public const string Schema = "Lobbies";

    public DbSet<Player> Players { get; set; }

    public DbSet<Lobby> Lobbies { get; set; }

    public LobbiesDbContext(DbContextOptions<LobbiesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LobbiesDbContext).Assembly);
    }
}