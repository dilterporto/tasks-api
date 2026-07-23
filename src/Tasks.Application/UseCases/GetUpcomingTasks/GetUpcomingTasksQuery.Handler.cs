using MapsterMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Domain.Projections;

namespace Tasks.Application.UseCases.GetUpcomingTasks;

public class GetUpcomingTasksQueryHandler(
  IProjectionsReader<TaskProjection> projectionsReader,
  IMapper mapper,
  ICacheManager cacheManager,
  ILogger<GetUpcomingTasksQueryHandler> logger)
  : IRequestHandler<GetUpcomingTasksQuery, Result<UpcomingTasksResponse>>
{
  public async Task<Result<UpcomingTasksResponse>> Handle(GetUpcomingTasksQuery request, CancellationToken cancellationToken)
  {
    var (tasks, fromCache) = await TryGetUpcomingTasks();

    if (!fromCache)
    {
      logger.LogInformation("[Application] Setting upcoming tasks in cache.");
      await cacheManager.Set(Constants.UpcomingTasksKey, tasks);
    }

    return GroupByDue(tasks);
  }

  private async Task<(List<TaskResponseWithDue> Tasks, bool FromCache)> TryGetUpcomingTasks()
  {
    if (await cacheManager.ContainsKey(Constants.UpcomingTasksKey))
    {
      var cached = await cacheManager.Get<IEnumerable<TaskResponseWithDue>>(Constants.UpcomingTasksKey);
      if (cached.HasValue)
        return (cached.Value.ToList(), true);

      logger.LogWarning("[Application] Cache key present but value unavailable, falling back to projections.");
    }

    return (await GetUpcomingTasksFromProjections(), false);
  }

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
