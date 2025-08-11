namespace ClinicApp.Domain.Common;

public abstract class AggregateRoot : Entity
{
    public AggregateRoot(Guid id) : base(id)
    {
    }
    protected AggregateRoot()
    {
    }

    protected List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();

        _domainEvents.Clear();
        return copy;
    }
}
