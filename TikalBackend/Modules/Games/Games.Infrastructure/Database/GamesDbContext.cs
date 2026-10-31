using Games.Application.DataAccess;
using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Database;

public sealed class GamesDbContext : DbContext, UnitOfWork
{
    public const string Schema = "Games";

    public DbSet<Player> Players { get; set; }

    public DbSet<Game> Games { get; set; }

    public DbSet<Tile> Tiles { get; set; }

    public DbSet<TroopAssignment> TroopAssignments { get; set; }

    public GamesDbContext(DbContextOptions<GamesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesDbContext).Assembly);
    }
}