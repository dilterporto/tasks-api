namespace Tasks.Domain.Aggregates.Tasks;

public partial class TaskAggregate
{
  public void Delete(string reason) =>
    ApplyChange(new TaskDeletedEvent
    {
      TaskId = this.State.Id,
      Reason = reason
    });
  
  public void Apply(TaskDeletedEvent @event) =>
    base.MarkAsRemoved();
}
