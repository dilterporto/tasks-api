using FastEndpoints;
using MediatR;
using Tasks.Api.Apis.Tasks.Messages;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.ChangeTask;

namespace Tasks.Api.Apis.Tasks;

public class PutTaskEndpoint(IMediator mediator) : Endpoint<ChangeTaskRequest, TaskResponse>
{
  public override void Configure()
  {
    Put("api/tasks/{id}");
    AllowAnonymous();
  }
  
  public override async Task HandleAsync(ChangeTaskRequest request, CancellationToken cancellationToken)
  {
    ChangeTaskCommand command = new ChangeTaskCommand(
      Route<Guid>("id"), 
      request.Subject, 
      request.Description, 
      request.UserId);
    
    var result = await mediator.Send(command, cancellationToken);
    
    await SendAsync(result.IsSuccess ?
      result.Value! : null!, cancellation: cancellationToken);
  }
}
