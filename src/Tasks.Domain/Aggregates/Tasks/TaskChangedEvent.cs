using Tasks.Abstractions;

namespace Tasks.Domain.Aggregates.Tasks;

public class TaskChangedEvent : DomainEvent
{
  public string? Subject { get; set; }
  public string? Description { get; set; }
  public Guid UserId { get; set; }
  public TaskChangedEvent() => this.Id = Guid.NewGuid();
}
