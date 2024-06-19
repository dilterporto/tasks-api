using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Persistence.Reading.Projections;

namespace Tasks.Application.UseCases.GetTaskById;

public class GetTaskByIdQueryHandler(IProjectionsReader<TaskProjection> projectionsReader, IMapper mapper) 
  : IRequestHandler<GetTaskByIdQuery, Result<TaskResponse>>
{
  public async Task<Result<TaskResponse>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
  {
    var getTaskByIdResult = await projectionsReader.GetByIdAsync(request.TaskId);
    
    return getTaskByIdResult.HasNoValue ?
      Result.Failure<TaskResponse>("Task not found") : mapper.Map<TaskResponse>(getTaskByIdResult.Value);
  }
}
