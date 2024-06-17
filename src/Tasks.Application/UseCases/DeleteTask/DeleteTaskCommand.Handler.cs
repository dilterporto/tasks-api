using CSharpFunctionalExtensions;
using Tasks.Abstractions.CQRS;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.UseCases.DeleteTask;

public class DeleteTaskCommandHandler(ITaskRepository taskRepository) : ICommandHandler<DeleteTaskCommand, Result>
{
  public async Task<Result> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
  {
    var aggregate = await taskRepository.LoadByIdAsync(command.TaskId);
    if (aggregate.HasNoValue)
    {
      return Result.Failure($"Task with ID {command.TaskId} was not found.");
    }

    var task = aggregate.Value;
    task.Delete("Deleted by user.");
    
    await taskRepository.SaveAsync(aggregate.Value);

    return Result.Success();
  }
}
