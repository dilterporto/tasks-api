using CSharpFunctionalExtensions;
using Tasks.Abstractions.CQRS;
using Tasks.Application.Contracts;

namespace Tasks.Application.UseCases.ChangeTask;

public record ChangeTaskCommand(Guid TaskId, string Subject, string Description, Guid UserId)
  : ICommand<Result<TaskResponse>>;


