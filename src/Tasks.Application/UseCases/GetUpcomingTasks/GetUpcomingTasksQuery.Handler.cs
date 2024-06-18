using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Persistence.Reading.Projections;

namespace Tasks.Application.UseCases.GetUpcomingTasks;

public class GetUpcomingTasksQueryHandler(IProjectionsReader<TaskProjection> projectionsReader, IMapper mapper, ICacheManager cacheManager) 
  : IRequestHandler<GetUpcomingTasksQuery, Result<UpcomingTasksResponse>>
{
  public async Task<Result<UpcomingTasksResponse>> Handle(GetUpcomingTasksQuery request, CancellationToken cancellationToken)
  {
    if (cacheManager.ContainsKey(Constants.UpcomingTasksKey)) 
      return GroupByDue(await GetUpcomingTasksFromCache());
    
    var upcomingTasksFromProjections = await GetUpcomingTasksFromProjections();
    
    await SetInCache(upcomingTasksFromProjections);
    
    return GroupByDue(upcomingTasksFromProjections);
  }

  private Task SetInCache(IEnumerable<TaskResponseWithDue> upcomingTasks) =>
    cacheManager.Set(Constants.UpcomingTasksKey, upcomingTasks);
  
  private async Task<List<TaskResponseWithDue>> GetUpcomingTasksFromCache() =>
    (await cacheManager.Get<IEnumerable<TaskResponseWithDue>>(Constants.UpcomingTasksKey)).Value.ToList();
  
  private async Task<List<TaskResponseWithDue>> GetUpcomingTasksFromProjections() =>
    (await projectionsReader.GetAllAsync())
    .Where(x => x.DueAt.Date >= DateTime.UtcNow.Date)
    .ToList()
    .Select(mapper.Map<TaskResponseWithDue>)
    .ToList();

  private static Result<UpcomingTasksResponse> GroupByDue(List<TaskResponseWithDue> upcomingTasks) =>
    new UpcomingTasksResponse(upcomingTasks
      .GroupBy(x => x.Due)
      .Select(x => new GroupedTaskResponse(x.Key, x
        .GroupBy(g => g.DueAt.Date)
        .Select(y => new DateGroupedTaskResponse(y.Key, y.Select(z => new TaskResponse(Id: z.Id, Status: z.Status,
          DueAt: z.DueAt, At: z.At,
          UserId: z.UserId, Description: z.Description, Subject: z.Subject, StartedAt: z.StartedAt,
          CompletedAt: z.CompletedAt)).ToList())).ToList()))
      .ToList());
}
