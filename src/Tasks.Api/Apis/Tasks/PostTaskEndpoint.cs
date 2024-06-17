using FastEndpoints;
using MediatR;
using Tasks.Api.Apis.Tasks.Messages;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.CreateTask;
using IMapper = AutoMapper.IMapper;

namespace Tasks.Api.Apis.Tasks;

public class PostTaskEndpoint(IMediator mediator, IMapper mapper) : Endpoint<CreateTaskRequest, TaskResponse>
{
  public override void Configure()
  {
    Post("api/tasks");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateTaskRequest request, CancellationToken cancellationToken)
  {
    CreateTaskCommand command = mapper.Map<CreateTaskCommand>(request);

    var result = await mediator.Send(command, cancellationToken);

    await SendAsync(result.IsSuccess ?
      result.Value! : null!, cancellation: cancellationToken);
  }
}

