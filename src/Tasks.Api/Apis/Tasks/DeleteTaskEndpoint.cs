using FastEndpoints;
using MediatR;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.DeleteTask;

namespace Tasks.Api.Apis.Tasks;

public class DeleteTaskEndpoint(IMediator mediator) : EndpointWithoutRequest<TaskResponse>
{
  public override void Configure()
  {
    Delete("api/tasks/{id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    DeleteTaskCommand command = new DeleteTaskCommand
    {
      TaskId = Route<Guid>("id")
    };

    var result = await mediator.Send(command, cancellationToken);

    if (result.IsFailure)
      await SendErrorsAsync(400, cancellationToken);

    await SendOkAsync(cancellationToken);
  }
}
