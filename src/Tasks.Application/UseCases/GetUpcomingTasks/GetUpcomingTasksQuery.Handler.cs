using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Persistence.Projections;

namespace Tasks.Application.UseCases.GetUpcomingTasks;

public class GetUpcomingTasksQueryHandler(IProjectionsReader<TaskProjection> projectionsReader, IMapper mapper) 
  : IRequestHandler<GetUpcomingTasksQuery, Result<UpcomingTasksResponse>>
{
  public async Task<Result<UpcomingTasksResponse>> Handle(GetUpcomingTasksQuery request, CancellationToken cancellationToken)
  {
    var upcomingTasks = (await projectionsReader.GetAllAsync())
      .Where(x => x.DueAt.Date >= DateTime.UtcNow.Date)
      .ToList()
      .Select(mapper.Map<TaskResponseWithDue>);

    var upcomingGroupedTasks = upcomingTasks
      .GroupBy(x => x.Due)
      .Select(x => new GroupedTaskResponse(x.Key, x
        .GroupBy(g => g.DueAt.Date)
        .Select(y => new DateGroupedTaskResponse(y.Key, y.Select(z => new TaskResponse(Id: z.Id, Status: z.Status,
          DueAt: z.DueAt, At: z.At,
          UserId: z.UserId, Description: z.Description, Subject: z.Subject, StartedAt: z.StartedAt,
          CompletedAt: z.CompletedAt)).ToList())).ToList()))
      .ToList();
    
    return new UpcomingTasksResponse(upcomingGroupedTasks);
  }
}
