namespace Tasks.Application.Contracts;

public class TaskResponse
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public DateTime At { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public string Subject { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; }
}
