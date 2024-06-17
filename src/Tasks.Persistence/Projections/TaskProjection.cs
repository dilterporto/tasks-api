using Tasks.Abstractions.EventSourcing;

namespace Tasks.Persistence.Projections;

public class TaskProjection : IProjection
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public DateTime At { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public DateTime DueAt { get; set; }
  public string Subject { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string? Status { get; set; }
}
