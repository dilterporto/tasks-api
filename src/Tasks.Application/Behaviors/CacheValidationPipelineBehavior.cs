using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.CQRS;

namespace Tasks.Application.Behaviors;

public class CacheValidationPipelineBehavior<TRequest, TResponse>(ICacheManager cacheManager, ILogger<CacheValidationPipelineBehavior<TRequest, TResponse>> logger) : CommandPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  public override async Task<TResponse> Handle(ICommand<TResponse> command, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    await cacheManager.Invalidate(Constants.UpcomingTasksKey);
    logger.LogWarning("[Application] Cache invalidated for key {Key}", Constants.UpcomingTasksKey);
    return await next();
  }
}
