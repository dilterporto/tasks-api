using MediatR;

namespace Tasks.Abstractions.CQRS;

public abstract class CommandPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  public abstract Task<TResponse> Handle(ICommand<TResponse> command, RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken);

  public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
  {
    if (request is ICommand<TResponse> command)
    {
      return await Handle(command, next, cancellationToken);
    }

    return await next();
  }
}
