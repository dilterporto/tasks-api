using Tasks.Abstractions;

namespace Tasks.Domain.Aggregates.Tasks;

public class TaskStartedEvent : DomainEvent
{
  public DateTime StartedAt { get; set; }
  public Guid AssignedTo { get; set; }
  public TaskStartedEvent() => this.Id = Guid.NewGuid();
}
