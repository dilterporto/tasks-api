using CSharpFunctionalExtensions;

namespace Tasks.Domain.Aggregates.Tasks;

public interface ITaskRepository
{
  Task<Maybe<TaskAggregate>> LoadByIdAsync(Guid id);
  Task SaveAsync(TaskAggregate aggregate);
}
