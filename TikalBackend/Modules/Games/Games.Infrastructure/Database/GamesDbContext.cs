using System.Transactions;
using Games.Application.DataAccess;
using Games.Domain.Entities;
using Games.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace Games.Infrastructure.Database;

public sealed class GamesDbContext : DbContext, UnitOfWork
{
    public const string Schema = "Games";

    private readonly IMediator mediator;

    public DbSet<Player> Players { get; set; }

    public DbSet<Game> Games { get; set; }

    public DbSet<TileMap> TileMaps { get; set; }

    public DbSet<Tile> Tiles { get; set; }

    public DbSet<TroopAssignment> TroopAssignments { get; set; }


    public GamesDbContext(DbContextOptions<GamesDbContext> options, IMediator mediator) : base(options)
    {
        this.mediator = mediator;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled
        );

        var result = await base.SaveChangesAsync(cancellationToken);

        await mediator.DispatchDomainEventsAsync(entitiesWithEvents);

        transaction.Complete();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesDbContext).Assembly);
    }
}