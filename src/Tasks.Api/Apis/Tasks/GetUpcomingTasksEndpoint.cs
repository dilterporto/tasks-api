using FastEndpoints;
using MediatR;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.GetUpcomingTasks;

namespace Tasks.Api.Apis.Tasks;

public class GetUpcomingTasksEndpoint(IMediator mediator) : 
  EndpointWithoutRequest<UpcomingTasksResponse>
{
  public override void Configure()
  {
    Get("api/tasks/upcoming");
    AllowAnonymous();
  }
  
  public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUpcomingTasksQuery(), cancellationToken);
        await SendAsync(result.IsSuccess ?
          result.Value! : null!, cancellation: cancellationToken);
    }
}
