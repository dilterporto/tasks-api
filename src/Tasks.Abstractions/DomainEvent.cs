namespace Tasks.Abstractions;

public abstract class DomainEvent : IDomainEvent
{
  public Guid Id { get; set; }
  public Guid AggregateId { get; set; }
  public long Version { get; set; } = -1;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
