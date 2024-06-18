using ReflectionMagic;

namespace Tasks.Abstractions.Domain;

public class AggregateRoot : IAggregateRoot
{
  public Guid Id { get; set; }
  public long Version { get; set; } = -1;
  public List<DomainEvent> UncommittedEvents { get; } = new();
  public bool Removed { get; set; }
  protected AggregateRoot() { }
  protected AggregateRoot(Guid id)
      => Id = id;

  public void MarkChangesAsCommitted()
      => UncommittedEvents.Clear();

  protected void ApplyChange(DomainEvent domainEvent)
      => ApplyChange(domainEvent, true, this.Version);

  public void ApplyChange(DomainEvent @event, bool isNew, long version = default)
  {
    if (isNew)
    {
      @event.Version = version + 1;
      @event.AggregateId = Id;
      UncommittedEvents.Add(@event);
    }
    this.Version = @event.Version;
    this.AsDynamic().Apply(@event);
  }

  protected void MarkAsRemoved() =>
    this.Removed = true;

}
