using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;


namespace Tasks.Abstractions.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

  public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
  {
    _logger = logger;
  }
  
  public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
  {
    Log.ForContext<TRequest>();
    Log.Information("[Application] Processing {Commmand}", request);
    return await next();
  }
}
