using FastEndpoints;
using MediatR;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.GetTaskById;

namespace Tasks.Api.Apis.Tasks;

public class GetTaskByIdEndpoint(IMediator mediator) : EndpointWithoutRequest<TaskResponse>
{
  public override void Configure()
  {
    Get("api/tasks/{id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    Guid id = Route<Guid>("id");

    var result = await mediator.Send(new GetTaskByIdQuery(id), cancellationToken);

    await SendAsync(result.IsSuccess ?
      result.Value! : null!, cancellation: cancellationToken);
  }
}
