using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Domain.Entities;

namespace Lobbies.Infrastructure.Database.Interceptors;

/// <summary>
/// Marks touched entities as modified so an UPDATE is issued for their row, even when only child entities changed.
/// This bumps the database maintained concurrency token (xmin) and runs the concurrency check.
/// </summary>
internal sealed class TouchedEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        MarkTouchedEntitiesAsModified(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        MarkTouchedEntitiesAsModified(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void MarkTouchedEntitiesAsModified(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var touchedEntries = context.ChangeTracker.Entries<Entity>()
            .Where(entry => entry.Entity.IsTouched && entry.State == EntityState.Unchanged);

        foreach (var entry in touchedEntries)
        {
            entry.State = EntityState.Modified;
        }
    }
}