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
      var aggregate = await taskRepository.LoadByIdAsync(command.TaskId);
      if (aggregate.HasNoValue)
      {
        return Result.Failure<TaskResponse>($"Task with id {command.TaskId} not found");
      }

      var newState = mapper.Map<TaskAggregateState>(command);
      var task = aggregate.Value;
      task.Change(newState);

      await taskRepository.SaveAsync(task);

      return Result.Success(mapper.Map<TaskResponse>(task.State));
    }
    catch (Exception e)
    {
      logger.LogError("An error occurred while changing the task: {Message}", e.Message);
      return Result.Failure<TaskResponse>($"An error occurred while changing the task: {e.Message}");
    }
  }
}
