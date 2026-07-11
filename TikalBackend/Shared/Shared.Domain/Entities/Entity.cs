using MediatR;

namespace Shared.Domain.Entities;

public abstract class Entity
{
    private readonly List<INotification> domainEvents = [];

    public IReadOnlyList<INotification> DomainEvents => domainEvents.AsReadOnly();

    protected void AddDomainEvent(INotification domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}