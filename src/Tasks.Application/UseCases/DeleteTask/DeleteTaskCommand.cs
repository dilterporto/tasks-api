using CSharpFunctionalExtensions;
using Tasks.Abstractions.CQRS;

namespace Tasks.Application.UseCases.DeleteTask;

public class DeleteTaskCommand : ICommand<Result>
{
  public Guid TaskId { get; set; }
}
