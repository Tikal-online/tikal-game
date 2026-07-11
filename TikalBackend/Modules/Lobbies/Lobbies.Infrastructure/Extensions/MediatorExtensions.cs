using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace Lobbies.Infrastructure.Extensions;

internal static class MediatorExtensions
{
    extension(IMediator mediator)
    {
        public async Task DispatchDomainEventsAsync(DbContext dbContext)
        {
            var entitiesWithEvents = dbContext.ChangeTracker
                .Entries<Entity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Count != 0)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}