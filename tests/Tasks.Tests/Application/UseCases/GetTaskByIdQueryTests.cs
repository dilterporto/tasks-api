using AutoFixture;
using AutoMapper;
using CSharpFunctionalExtensions;
using Moq;
using Tasks.Abstractions.EventSourcing;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.GetTaskById;
using Tasks.Persistence.Reading.Projections;
using Xunit;

namespace Tasks.Tests.Application.UseCases;

public class GetTaskByIdQueryTests
{
  private readonly GetTaskByIdQueryHandler _getTaskByIdQueryHandler;
  
  public GetTaskByIdQueryTests()
  {
    var projectionsReader = new Mock<IProjectionsReader<TaskProjection>>();
    var mapper = new Mock<IMapper>();
    
    _getTaskByIdQueryHandler = new GetTaskByIdQueryHandler(projectionsReader.Object, mapper.Object);  
  }
  
  [Fact]
  public async Task Handle_WhenTaskNotFound_ReturnsFailure()
  {
    // Arrange
    var taskId = Guid.NewGuid();
    var query = new GetTaskByIdQuery(taskId);
    
    var task = Maybe<TaskProjection>.None;
    var expectedResult = Result.Failure<TaskResponse>("Task not found");
    
    var projectionsReader = new Mock<IProjectionsReader<TaskProjection>>();
    projectionsReader.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(task);
    
    var mapper = new Mock<IMapper>();
    
    var handler = new GetTaskByIdQueryHandler(projectionsReader.Object, mapper.Object);
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    Assert.Equal(expectedResult, result);
  }
  
  [Fact]
  public async Task Handle_WhenTaskFound_ReturnsTaskResponse()
  {
    // Arrange
    var taskId = Guid.NewGuid();
    var query = new GetTaskByIdQuery(taskId);
    
    var task = new TaskProjection();
    var expectedResult = new Fixture().Create<TaskResponse>();
    
    var projectionsReader = new Mock<IProjectionsReader<TaskProjection>>();
    projectionsReader.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(Maybe<TaskProjection>.From(task));
    
    var mapper = new Mock<IMapper>();
    mapper.Setup(x => x.Map<TaskResponse>(task)).Returns(expectedResult);
    
    var handler = new GetTaskByIdQueryHandler(projectionsReader.Object, mapper.Object);
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    Assert.Equal(Result.Success(expectedResult), result);
  }
}
