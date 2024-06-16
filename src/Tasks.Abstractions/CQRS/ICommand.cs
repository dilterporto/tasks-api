using MediatR;

namespace Tasks.Abstractions.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>
{

}
