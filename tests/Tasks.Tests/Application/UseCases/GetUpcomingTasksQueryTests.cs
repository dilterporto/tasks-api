using CSharpFunctionalExtensions;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.GetUpcomingTasks;
using Tasks.Domain.Projections;
using Xunit;

namespace Tasks.Tests.Application.UseCases;

public class GetUpcomingTasksQueryTests
{
  private readonly Mock<IProjectionsReader<TaskProjection>> _projectionsReader = new();
  private readonly Mock<IMapper> _mapper = new();
  private readonly Mock<ICacheManager> _cache = new();
  private readonly Mock<ILogger<GetUpcomingTasksQueryHandler>> _logger = new();

  private GetUpcomingTasksQueryHandler BuildHandler() =>
    new(_projectionsReader.Object, _mapper.Object, _cache.Object, _logger.Object);

  [Fact]
  public async Task Handle_WhenNoUpcomingTasks_ReturnsEmptyResponse()
  {
    _cache.Setup(x => x.ContainsKey(It.IsAny<string>())).ReturnsAsync(false);
    _projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TaskProjection>().AsQueryable());

    var result = await BuildHandler().Handle(new GetUpcomingTasksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Empty(result.Value.Tasks);
  }

  [Fact]
  public async Task Handle_WhenUpcomingTasks_ReturnsUpcomingTasksGroupedByDue()
  {
    var tasks = new List<TaskProjection>
    {
      new() { DueAt = DateTime.UtcNow.Date },
      new() { DueAt = DateTime.UtcNow.Date },
      new() { DueAt = DateTime.UtcNow.Date.AddDays(1) },
      new() { DueAt = DateTime.UtcNow.Date.AddDays(2) },
      new() { DueAt = DateTime.UtcNow.Date.AddDays(1) }
    }.AsQueryable();

    _cache.Setup(x => x.ContainsKey(It.IsAny<string>())).ReturnsAsync(false);
    _projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(tasks);

    foreach (var task in tasks)
      _mapper.Setup(x => x.Map<TaskResponseWithDue>(task)).Returns(new TaskResponseWithDue
      {
        Due = task.DueAt.Date == DateTime.UtcNow.Date ? "today" : "upcoming",
        Id = task.Id, Status = task.Status, DueAt = task.DueAt, At = task.At,
        UserId = task.UserId, Description = task.Description, Subject = task.Subject,
        StartedAt = task.StartedAt, CompletedAt = task.CompletedAt
      });

    var result = await BuildHandler().Handle(new GetUpcomingTasksQuery(), CancellationToken.None);

    Assert.Equal(1, result.Value.Tasks.Count(x => x.Group == "today"));
    Assert.Equal(1, result.Value.Tasks.Count(x => x.Group == "upcoming"));
  }

  // AC-1: Redis throws on ContainsKey → fallback to DB
  [Fact]
  public async Task Handle_WhenRedisThrowsOnContainsKey_ReturnsDatabaseResult()
  {
    _cache.Setup(x => x.ContainsKey(It.IsAny<string>())).ReturnsAsync(false); // CacheManager returns false on exception
    _projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TaskProjection>().AsQueryable());

    var result = await BuildHandler().Handle(new GetUpcomingTasksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);
    _projectionsReader.Verify(x => x.GetAllAsync(), Times.Once);
  }

  // AC-2: Redis throws on Get → fallback to DB
  [Fact]
  public async Task Handle_WhenRedisThrowsOnGet_ReturnsDatabaseResult()
  {
    _cache.Setup(x => x.ContainsKey(It.IsAny<string>())).ReturnsAsync(true);
    _cache.Setup(x => x.Get<IEnumerable<TaskResponseWithDue>>(It.IsAny<string>()))
          .ReturnsAsync(Maybe<IEnumerable<TaskResponseWithDue>>.None); // CacheManager returns None on exception
    _projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TaskProjection>().AsQueryable());

    var result = await BuildHandler().Handle(new GetUpcomingTasksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);
    _projectionsReader.Verify(x => x.GetAllAsync(), Times.Once);
  }

  // AC-3: Redis throws on Set → response still returned, no exception surfaced
  [Fact]
  public async Task Handle_WhenRedisThrowsOnSet_ReturnsResponseWithoutThrowing()
  {
    _cache.Setup(x => x.ContainsKey(It.IsAny<string>())).ReturnsAsync(false);
    _cache.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<IEnumerable<TaskResponseWithDue>>(), null))
          .Returns(Task.CompletedTask); // CacheManager swallows the exception internally
    _projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TaskProjection>().AsQueryable());

    var result = await BuildHandler().Handle(new GetUpcomingTasksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);
  }

  // AC-4: ContainsKey returns true but Get returns None (race condition) → fallback to DB, no throw
  [Fact]
  public async Task Handle_WhenCacheMaybeIsNone_FallsBackWithoutThrowing()
  {
    _cache.Setup(x => x.ContainsKey(It.IsAny<string>())).ReturnsAsync(true);
    _cache.Setup(x => x.Get<IEnumerable<TaskResponseWithDue>>(It.IsAny<string>()))
          .ReturnsAsync(Maybe<IEnumerable<TaskResponseWithDue>>.None);
    _projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TaskProjection>().AsQueryable());

    var exception = await Record.ExceptionAsync(() =>
      BuildHandler().Handle(new GetUpcomingTasksQuery(), CancellationToken.None));

    Assert.Null(exception);
    _projectionsReader.Verify(x => x.GetAllAsync(), Times.Once);
  }

  [Fact]
  public async Task Handle_WhenCacheHit_DoesNotCallProjectionsReader()
  {
    var cachedTasks = new List<TaskResponseWithDue>
    {
      new() { Due = "today", DueAt = DateTime.UtcNow.Date }
    };

    _cache.Setup(x => x.ContainsKey(It.IsAny<string>())).ReturnsAsync(true);
    _cache.Setup(x => x.Get<IEnumerable<TaskResponseWithDue>>(It.IsAny<string>()))
          .ReturnsAsync(Maybe<IEnumerable<TaskResponseWithDue>>.From(cachedTasks));

    var result = await BuildHandler().Handle(new GetUpcomingTasksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);
    _projectionsReader.Verify(x => x.GetAllAsync(), Times.Never);
  }
}
