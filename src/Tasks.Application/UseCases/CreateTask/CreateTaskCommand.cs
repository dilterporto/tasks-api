using CSharpFunctionalExtensions;
using Tasks.Abstractions.CQRS;
using Tasks.Application.Contracts;

namespace Tasks.Application.UseCases.CreateTask;

public record CreateTaskCommand(Guid UserId, string Subject, string Description, DateTime DueAt)
  : ICommand<Result<TaskResponse>>;
