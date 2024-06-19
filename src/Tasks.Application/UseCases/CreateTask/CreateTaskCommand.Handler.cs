using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Tasks.Abstractions.CQRS;
using Tasks.Application.Contracts;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.UseCases.CreateTask;

public class CreateTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper, ILogger<CreateTaskCommandHandler> logger)
  : ICommandHandler<CreateTaskCommand, Result<TaskResponse>>
{
  public async Task<Result<TaskResponse>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var state = mapper.Map<TaskAggregateState>(command);
      TaskAggregate taskAggregate = new TaskAggregate(state);
      
      taskAggregate.Start(command.UserId);
      
      await taskRepository.SaveAsync(taskAggregate);
      return mapper.Map<TaskResponse>(taskAggregate.State);
    }
    catch
    {
      logger.LogError("An error occurred while creating the task.");
      return Result.Failure<TaskResponse>("An error occurred while creating the task.");
    }
  }
}
