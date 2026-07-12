using MediatR;
using Shared.Domain.Entities;

namespace Lobbies.Infrastructure.Extensions;

internal static class MediatorExtensions
{
    extension(IMediator mediator)
    {
        public async Task DispatchDomainEventsAsync(List<Entity> entitiesWithEvents)
        {
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