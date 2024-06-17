using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tasks.Application.Contracts;
using Tasks.Domain.Aggregates.Tasks;
using Tasks.Persistence.Projections;

namespace Tasks.Application.UseCases.GetTaskById;

public class GetTaskByIdQueryHandler(ProjectionsDbContext projectionsDbContext, IMapper mapper) : IRequestHandler<GetTaskByIdQuery, Result<TaskResponse>>
{
  public async Task<Result<TaskResponse>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
  {
    var task = await projectionsDbContext
      .Set<TaskProjection>()
      .TagWith("GetTaskByIdQuery - TaskProjection")
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken: cancellationToken);
    
    return task == null ?
      Result.Failure<TaskResponse>("Task not found") : mapper.Map<TaskResponse>(task);
  }
}
