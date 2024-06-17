namespace Tasks.Domain.Aggregates.Tasks;

public partial class TaskAggregate
{
  public void Change(TaskAggregateState state) =>
    ApplyChange(new TaskChangedEvent
    { 
      Subject = state.Subject,
      Description = state.Description,
      UserId = state.UserId
    });

  public void Apply(TaskChangedEvent @event)
  {
    this.State.Subject = @event.Subject!;
    this.State.Description = @event.Description!;
    this.State.UserId = @event.UserId;
  }
}
