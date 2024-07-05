using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Persistence.Reading.Projections;

namespace Tasks.Application.UseCases.GetUpcomingTasks;

public class GetUpcomingTasksQueryHandler(IProjectionsReader<TaskProjection> projectionsReader, IMapper mapper, ICacheManager cacheManager, ILogger<GetUpcomingTasksQueryHandler> logger) 
  : IRequestHandler<GetUpcomingTasksQuery, Result<UpcomingTasksResponse>>
{
  public async Task<Result<UpcomingTasksResponse>> Handle(GetUpcomingTasksQuery request, CancellationToken cancellationToken) =>
    await TryGetUpcomingTasks()
      .ToResult("An error occurred while getting upcoming tasks.")
      .Map(upcomingTasks =>
      {
        if (IsCacheEmpty().IsSuccess)
          SetInCache(upcomingTasks);
        return upcomingTasks;
      })
      .Finally(upcomingTasksResult => GroupByDue(upcomingTasksResult.Value));

  private async Task<Maybe<List<TaskResponseWithDue>>> TryGetUpcomingTasks() =>
    cacheManager.ContainsKey(Constants.UpcomingTasksKey) ? 
      await GetUpcomingTasksFromCache() : await GetUpcomingTasksFromProjections();
  
  private void SetInCache(IEnumerable<TaskResponseWithDue> upcomingTasks)
  {
    logger.LogInformation("[Application] Setting upcoming tasks in cache.");
    cacheManager.Set(Constants.UpcomingTasksKey, upcomingTasks);
  }

  private Result IsCacheEmpty() => 
    cacheManager.ContainsKey(Constants.UpcomingTasksKey) ? Result.Failure("Cache does not exist.") : Result.Success();

  private async Task<Maybe<List<TaskResponseWithDue>>> GetUpcomingTasksFromCache() =>
    (await cacheManager.Get<IEnumerable<TaskResponseWithDue>>(Constants.UpcomingTasksKey)).Value.ToList();
  
  private async Task<Maybe<List<TaskResponseWithDue>>> GetUpcomingTasksFromProjections() =>
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
