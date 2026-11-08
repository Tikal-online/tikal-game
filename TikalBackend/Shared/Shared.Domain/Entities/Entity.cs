namespace Shared.Domain.Entities;

public abstract class Entity
{
    private readonly List<IDomainEvent> domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    private bool isTouched;

    public bool IsTouched => isTouched;

    public void Touch()
    {
        isTouched = true;
    }
}