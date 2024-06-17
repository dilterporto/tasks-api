using Tasks.Abstractions;

namespace Tasks.Domain.Aggregates.Tasks;

public partial class TaskAggregate : AggregateRoot
{
  public ITaskAggregateState State { get; set; } = new TaskAggregateState();

  public TaskAggregate() { }

  public TaskAggregate(Guid id) : base(id) { }

  public TaskAggregate(ITaskAggregateState state) : base(Guid.NewGuid()) =>
    ApplyChange(new TaskCreatedEvent
    {
      UserId = state.UserId,
      At = state.At,
      Subject = state.Subject,
      Description = state.Description
    });

  public void Apply(TaskCreatedEvent @event)
  {
    State.Id = @event.AggregateId;
    State.UserId = @event.UserId;
    State.At = @event.At;
    State.Subject = @event.Subject;
    State.Description = @event.Description;
    State.Status = TaskStatus.Created;
  }
}
