namespace Tasks.Abstractions;

public interface IAggregateRoot
{
  public Guid Id { get; set; }
  public long Version { get; set; }
  void MarkChangesAsCommitted();
  List<DomainEvent> UncommittedEvents { get; }
}
