namespace Tasks.Domain.Aggregates.Tasks;

public interface ITaskAggregateState
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public DateTime At { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public string Subject { get; set; }
  public string Description { get; set; }
  public TaskStatus Status { get; set; }
  public bool Deleted { get; set; }
  public DateTime DueAt { get; set; }
}

public class TaskAggregateState : ITaskAggregateState
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public DateTime At { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public string Subject { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; }
  public bool Deleted { get; set; }
  public DateTime DueAt { get; set; }
}
