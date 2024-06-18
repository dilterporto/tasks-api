using Tasks.Abstractions.Domain;

namespace Tasks.Domain.Aggregates.Tasks;

public partial class TaskAggregate : AggregateRoot, ITaskAggregate
{
  public ITaskAggregateState State { get; set; } = new TaskAggregateState();

  public TaskAggregate() { }

  public TaskAggregate(Guid id) : base(id) { }

  public TaskAggregate(ITaskAggregateState state) : base(Guid.NewGuid()) =>
    ApplyChange(new TaskCreatedEvent
    {
      At = state.At,
      DueAt = state.DueAt,
      Subject = state.Subject,
      Description = state.Description
    });

  public void Apply(TaskCreatedEvent @event)
  {
    this.State.Id = @event.AggregateId;
    this.State.At = @event.At;
    this.State.Subject = @event.Subject;
    this.State.Description = @event.Description;
    this.State.DueAt = @event.DueAt;
    this.State.Status = TaskStatus.Created;
  }
}
