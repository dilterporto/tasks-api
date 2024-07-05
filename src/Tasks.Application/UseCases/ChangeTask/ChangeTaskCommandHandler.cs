using AutoMapper;
using CSharpFunctionalExtensions;
using Tasks.Abstractions.CQRS;
using Tasks.Application.Contracts;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.UseCases.ChangeTask;

public class ChangeTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper) : ICommandHandler<ChangeTaskCommand, Result<TaskResponse>>
{
  public async Task<Result<TaskResponse>> Handle(ChangeTaskCommand command, CancellationToken cancellationToken) =>
    await taskRepository.LoadByIdAsync(command.TaskId)
      .ToResult($"Task with id {command.TaskId} not found")
      .Tap(task => task.Change(mapper.Map<TaskAggregateState>(command)))
      .Check(taskRepository.SaveAsync)
      .Map(task => mapper.Map<TaskResponse>(task.State));
}
