using Lobbies.Application.DataAccess;
using Lobbies.Domain.Entities;
using Lobbies.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lobbies.Infrastructure.Database;

public sealed class LobbiesDbContext : DbContext, UnitOfWork
{
    public const string Schema = "Lobbies";

    private readonly IMediator mediator;

    public DbSet<Player> Players { get; set; }

    public DbSet<Lobby> Lobbies { get; set; }

    public LobbiesDbContext(DbContextOptions<LobbiesDbContext> options, IMediator mediator) : base(options)
    {
        this.mediator = mediator;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await mediator.DispatchDomainEventsAsync(this);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LobbiesDbContext).Assembly);
    }
}