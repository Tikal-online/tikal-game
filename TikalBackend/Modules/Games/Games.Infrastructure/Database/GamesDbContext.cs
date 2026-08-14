using Games.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Database;

public sealed class GamesDbContext : DbContext
{
    public const string Schema = "Games";

    private readonly IMediator mediator;

    public DbSet<Player> Players { get; set; }

    public DbSet<Game> Games { get; set; }

    public GamesDbContext(DbContextOptions<GamesDbContext> options, IMediator mediator) : base(options)
    {
        this.mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesDbContext).Assembly);
    }
}