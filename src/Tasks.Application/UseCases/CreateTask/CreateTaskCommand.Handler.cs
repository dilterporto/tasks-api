using AutoMapper;
using CSharpFunctionalExtensions;
using Tasks.Abstractions.CQRS;
using Tasks.Application.Contracts;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.UseCases.CreateTask;

public class CreateTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper)
  : ICommandHandler<CreateTaskCommand, Result<TaskResponse>>
{
  public Task<Result<TaskResponse>> Handle(CreateTaskCommand command, CancellationToken cancellationToken) =>
    CreateTaskAggregate(command)
      .ToResult("An error occurred while creating the task.")
      .TapIf(command.StartsAtCreation, task => task.Start(command.UserId))
      .Check(taskRepository.SaveAsync)
      .Map(task => mapper.Map<TaskResponse>(task.State));

  private Maybe<TaskAggregate> CreateTaskAggregate(CreateTaskCommand command) => 
    new TaskAggregate(mapper.Map<TaskAggregateState>(command));
}
