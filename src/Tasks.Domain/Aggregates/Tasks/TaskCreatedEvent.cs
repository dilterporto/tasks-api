using Tasks.Abstractions.Domain;

namespace Tasks.Domain.Aggregates.Tasks;

public class TaskCreatedEvent : DomainEvent
{
  public DateTime At { get; set; }
  public DateTime DueAt { get; set; }
  public string Subject { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskCreatedEvent() => Id = Guid.NewGuid();
}
