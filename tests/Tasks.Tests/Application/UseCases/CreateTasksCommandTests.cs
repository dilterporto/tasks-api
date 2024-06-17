using AutoFixture;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Moq;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.CreateTask;
using Tasks.Domain.Aggregates.Tasks;
using Xunit;

namespace Tasks.Tests.Application.UseCases;

public class CreateTasksCommandTests
{
  private readonly CreateTaskCommandHandler _createTasksCommandHandler;
  private readonly Mock<ITaskRepository> _taskRepository;
  private readonly Mock<IMapper> _mapper;

  public CreateTasksCommandTests()
  {
    _taskRepository = new Mock<ITaskRepository>();
    _mapper = new Mock<IMapper>();
    
    Mock<ILogger<CreateTaskCommandHandler>> logger = new();
    
    _createTasksCommandHandler = new CreateTaskCommandHandler(_taskRepository.Object, _mapper.Object, logger.Object);
  }
  
  [Fact] public async Task CreateTasksCommand_ShouldCreateWithSuccess()
  {
    // Arrange
    var createTasksCommand = new Fixture().Create<CreateTaskCommand>();
    
    _taskRepository
      .Setup(x => x.SaveAsync(It.IsAny<TaskAggregate>()))
      .Returns(Task.CompletedTask);

    _mapper
      .Setup(x => x.Map<TaskAggregateState>(It.IsAny<CreateTaskCommand>()))
      .Returns((TaskAggregateState)new TaskAggregate().State);
    
    // Act
    var result = await _createTasksCommandHandler.Handle(createTasksCommand, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.IsType<Result<TaskResponse>>(result);
    
    _taskRepository.Verify(x => x.SaveAsync(It.IsAny<TaskAggregate>()), Times.Once);
  }
  
  [Fact]
  public async Task CreateTasksCommand_ShouldCreateWithFailure()
  {
    // Arrange
    var createTasksCommand = new Fixture().Create<CreateTaskCommand>();
    
    _taskRepository
      .Setup(x => x.SaveAsync(It.IsAny<TaskAggregate>()))
      .Throws(new Exception());
    
    _mapper
      .Setup(x => x.Map<TaskAggregateState>(It.IsAny<CreateTaskCommand>()))
      .Returns((TaskAggregateState)new TaskAggregate().State);
    
    // Act
    var result = await _createTasksCommandHandler.Handle(createTasksCommand, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsFailure);
    Assert.IsType<Result<TaskResponse>>(result);
    
    _taskRepository.Verify(x => x.SaveAsync(It.IsAny<TaskAggregate>()), Times.Once);
  }
  
}
