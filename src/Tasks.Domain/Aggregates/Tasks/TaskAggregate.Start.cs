namespace Tasks.Domain.Aggregates.Tasks;

public partial class TaskAggregate
{
  public void Start(Guid userAssigned) =>
    ApplyChange(new TaskStartedEvent
    {
      AssignedTo = userAssigned,
      StartedAt = DateTime.UtcNow
    });

  public void Apply(TaskStartedEvent @event)
  {
    this.State.UserId = @event.AssignedTo;
    this.State.StartedAt = @event.StartedAt;
    this.State.Status = TaskStatus.Started;
  }
}
