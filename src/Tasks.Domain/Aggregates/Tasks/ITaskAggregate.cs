using Tasks.Abstractions.Domain;

namespace Tasks.Domain.Aggregates.Tasks;

public interface ITaskAggregate : IAggregateRoot
{
  void Change(TaskAggregateState state);
  void Delete(string reason);
  void Start(Guid userAssigned);
}
