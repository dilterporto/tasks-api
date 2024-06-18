using AutoMapper;
using Moq;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.GetUpcomingTasks;
using Tasks.Persistence.Reading.Projections;
using Xunit;

namespace Tasks.Tests.Application.UseCases;

public class GetUpcomingTasksQueryTests
{
  [Fact]
  public async Task Handle_WhenNoUpcomingTasks_ReturnsEmptyResponse()
  {
    // Arrange
    var query = new GetUpcomingTasksQuery();
    
    var tasks = new List<TaskProjection>().AsQueryable();
    var expectedResult = new UpcomingTasksResponse([]);
    
    var projectionsReader = new Mock<IProjectionsReader<TaskProjection>>();
    projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(tasks);
    
    var mapper = new Mock<IMapper>();
    
    var cache = new Mock<ICacheManager>();
    var handler = new GetUpcomingTasksQueryHandler(projectionsReader.Object, mapper.Object, cache.Object);
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    Assert.Empty(result.Value.Tasks);
  }
  
  [Fact]
  public async Task Handle_WhenUpcomingTasks_ReturnsUpcomingTasks()
  {
    // Arrange
    var query = new GetUpcomingTasksQuery();
    
    var tasks = new List<TaskProjection>
    {
      new() { DueAt = DateTime.UtcNow.Date },
      new() { DueAt = DateTime.UtcNow.Date },
      new() { DueAt = DateTime.UtcNow.Date.AddDays(1) },
      new() { DueAt = DateTime.UtcNow.Date.AddDays(2) },
      new() { DueAt = DateTime.UtcNow.Date.AddDays(1) }
    }.AsQueryable();
    
    var projectionsReader = new Mock<IProjectionsReader<TaskProjection>>();
    projectionsReader.Setup(x => x.GetAllAsync()).ReturnsAsync(tasks);
    
    var mapper = new Mock<IMapper>();

    foreach (var task in tasks)
    {
      mapper.Setup(x => x.Map<TaskResponseWithDue>(task)).Returns(new TaskResponseWithDue
      {
        Due = task.DueAt.Date == DateTime.UtcNow.Date ? "today" : "upcoming",
        Id = task.Id,
        Status = task.Status,
        DueAt = task.DueAt,
        At = task.At,
        UserId = task.UserId,
        Description = task.Description,
        Subject = task.Subject,
        StartedAt = task.StartedAt,
        CompletedAt = task.CompletedAt
      });
    }
    
    var cache = new Mock<ICacheManager>();
    var handler = new GetUpcomingTasksQueryHandler(projectionsReader.Object, mapper.Object, cache.Object);
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    Assert.Equal(1, result.Value.Tasks.Count(x => x.Group == "today"));
    Assert.Equal(1, result.Value.Tasks.Count(x => x.Group == "upcoming"));
  }
}
