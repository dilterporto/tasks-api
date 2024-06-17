using MediatR;
using Tasks.Abstractions.CQRS;
using Tasks.Persistence.Events;

namespace Tasks.Persistence;

public class UnitOfWorkBehavior<TRequest, TResponse>(EventsDbContext unitOfWork) : CommandPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  public override async Task<TResponse> Handle(ICommand<TResponse> command, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    var response = await next();
    await unitOfWork.SaveChangesAsync(cancellationToken);
    return response;
  }
}
