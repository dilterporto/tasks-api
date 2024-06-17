using Tasks.Abstractions;

namespace Tasks.Domain.Aggregates.Tasks;

public class TaskDeletedEvent : DomainEvent
{
  public Guid TaskId { get; set; }
  public string? Reason { get; set; }
  public TaskDeletedEvent() => Id = Guid.NewGuid();
}
