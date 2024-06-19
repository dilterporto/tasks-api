using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Tasks.Abstractions.CQRS;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.UseCases.DeleteTask;

public class DeleteTaskCommandHandler(ITaskRepository taskRepository, ILogger<DeleteTaskCommandHandler> logger) : ICommandHandler<DeleteTaskCommand, Result>
{
  public async Task<Result> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var loadTaskByIdResult = await taskRepository.LoadByIdAsync(command.TaskId);
      if (loadTaskByIdResult.HasNoValue)
      {
        return Result.Failure($"Task with ID {command.TaskId} was not found.");
      }

      var taskAggregate = loadTaskByIdResult.Value;
      taskAggregate.Delete("Deleted by user.");
    
      await taskRepository.SaveAsync(taskAggregate);

      return Result.Success();
    }
    catch (Exception e)
    {
      logger.LogError("An error occurred while changing the task: {Message}", e.Message);
      return Result.Failure($"An error occurred while deleting the task: {e.Message}");
    }
  }
}
