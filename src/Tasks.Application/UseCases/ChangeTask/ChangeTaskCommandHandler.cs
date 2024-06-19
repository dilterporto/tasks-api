using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Tasks.Abstractions.CQRS;
using Tasks.Application.Contracts;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.UseCases.ChangeTask;

public class ChangeTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper, ILogger<ChangeTaskCommandHandler> logger) : ICommandHandler<ChangeTaskCommand, Result<TaskResponse>>
{
  public async Task<Result<TaskResponse>> Handle(ChangeTaskCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var loadTaskByIdResult = await taskRepository.LoadByIdAsync(command.TaskId);
      if (loadTaskByIdResult.HasNoValue)
      {
        return Result.Failure<TaskResponse>($"Task with id {command.TaskId} not found");
      }

      var changedTaskAggregateState = mapper.Map<TaskAggregateState>(command);
      var taskAggregate = loadTaskByIdResult.Value;
      taskAggregate.Change(changedTaskAggregateState);

      await taskRepository.SaveAsync(taskAggregate);

      return Result.Success(mapper.Map<TaskResponse>(taskAggregate.State));
    }
    catch (Exception e)
    {
      logger.LogError("An error occurred while changing the task: {Message}", e.Message);
      return Result.Failure<TaskResponse>($"An error occurred while changing the task: {e.Message}");
    }
  }
}
