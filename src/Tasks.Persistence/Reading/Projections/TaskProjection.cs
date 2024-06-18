using Tasks.Abstractions.EventSourcing;

namespace Tasks.Persistence.Reading.Projections;

public class TaskProjection : Projection
{
  public Guid UserId { get; set; }
  public DateTime At { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public DateTime DueAt { get; set; }
  public string Subject { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string? Status { get; set; }
}
