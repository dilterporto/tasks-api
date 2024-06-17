using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Tasks.Application.Contracts;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.UseCases.GetTaskById;

public class GetTaskByIdQueryHandler(ITaskRepository taskRepository, IMapper mapper) : IRequestHandler<GetTaskByIdQuery, Result<TaskResponse>>
{
  public async Task<Result<TaskResponse>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
  {
    var task = await taskRepository.LoadByIdAsync(request.TaskId);
    return task.HasNoValue ?
      Result.Failure<TaskResponse>("Task not found") : mapper.Map<TaskResponse>(task.Value.State);
  }
}
