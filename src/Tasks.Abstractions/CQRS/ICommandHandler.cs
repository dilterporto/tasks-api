using MediatR;

namespace Tasks.Abstractions.CQRS;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
  where TCommand : IRequest<TResponse>
{

}
