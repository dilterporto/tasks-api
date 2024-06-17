using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tasks.Application.Contracts;
using Tasks.Persistence.Projections;

namespace Tasks.Application.UseCases.GetUpcomingTasks;

public class GetUpcomingTasksQueryHandler(ProjectionsDbContext projectionsDbContext, IMapper mapper) 
  : IRequestHandler<GetUpcomingTasksQuery, Result<UpcomingTasksResponse>>
{
  public async Task<Result<UpcomingTasksResponse>> Handle(GetUpcomingTasksQuery request, CancellationToken cancellationToken)
  {
    var upcomingTasks = (await projectionsDbContext
      .Set<TaskProjection>()
      .TagWith("GetUpcomingTasksQuery - UpcomingGroupedTasksProjection")
      .Where(x => x.DueAt.Date >= DateTime.UtcNow.Date)
      .AsNoTracking()
      .ToListAsync(cancellationToken: cancellationToken))
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
