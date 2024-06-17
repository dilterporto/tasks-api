using Tasks.Abstractions;

namespace Tasks.Domain.Aggregates.Tasks;

public class TaskCreatedEvent : DomainEvent
{
  public Guid UserId { get; set; }
  public DateTime At { get; set; }
  public string Subject { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskCreatedEvent() => Id = Guid.NewGuid();
}
