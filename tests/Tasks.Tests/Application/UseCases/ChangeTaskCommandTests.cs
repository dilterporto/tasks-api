using AutoFixture;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Moq;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.ChangeTask;
using Tasks.Domain.Aggregates.Tasks;
using Xunit;

namespace Tasks.Tests.Application.UseCases;

public class ChangeTaskCommandTests
{
  private readonly ChangeTaskCommandHandler _changeTaskCommandHandler;
  private readonly Mock<ITaskRepository> _taskRepository;
  private readonly Mock<IMapper> _mapper;

  public ChangeTaskCommandTests()
  {
    _taskRepository = new Mock<ITaskRepository>();
    _mapper = new Mock<IMapper>();
    _changeTaskCommandHandler = new ChangeTaskCommandHandler(_taskRepository.Object, _mapper.Object);  
  }
  
  [Fact]
  public async Task ChangeTaskCommand_ShouldChangeWithSuccess()
  {
    // Arrange
    var changeTaskCommand = new Fixture().Create<ChangeTaskCommand>();
    
    _taskRepository
      .Setup(x => x.LoadByIdAsync(It.IsAny<Guid>()))
      .ReturnsAsync(new TaskAggregate());
    
    _taskRepository
      .Setup(x => x.SaveAsync(It.IsAny<TaskAggregate>()))
      .ReturnsAsync(Result.Success);

    _mapper
      .Setup(x => x.Map<TaskAggregateState>(It.IsAny<ChangeTaskCommand>()))
      .Returns((TaskAggregateState)new TaskAggregate().State);
    
    // Act
    var result = await _changeTaskCommandHandler.Handle(changeTaskCommand, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.IsType<Result<TaskResponse>>(result);
    
    _taskRepository.Verify(x => x.LoadByIdAsync(It.IsAny<Guid>()), Times.Once);
    _taskRepository.Verify(x => x.SaveAsync(It.IsAny<TaskAggregate>()), Times.Once);
  }
  
  [Fact]
  public async Task ChangeTaskCommand_ShouldChangeWithFailure()
  {
    // Arrange
    var changeTaskCommand = new Fixture().Create<ChangeTaskCommand>();
    
    _taskRepository
      .Setup(x => x.LoadByIdAsync(It.IsAny<Guid>()))
      .ReturnsAsync(Maybe<TaskAggregate>.None);
    
    // Act
    var result = await _changeTaskCommandHandler.Handle(changeTaskCommand, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsFailure);
    Assert.IsType<Result<TaskResponse>>(result);
    
    _taskRepository.Verify(x => x.LoadByIdAsync(It.IsAny<Guid>()), Times.Once);
    _taskRepository.Verify(x => x.SaveAsync(It.IsAny<TaskAggregate>()), Times.Never);
  }
  
  [Fact]
  public async Task ChangeTaskCommand_ShouldChangeWithExceptionOnSave()
  {
    // Arrange
    var changeTaskCommand = new Fixture().Create<ChangeTaskCommand>();
    
    _taskRepository
      .Setup(x => x.LoadByIdAsync(It.IsAny<Guid>()))
      .ReturnsAsync(new TaskAggregate());
    
    _taskRepository
      .Setup(x => x.SaveAsync(It.IsAny<TaskAggregate>()))
      .ReturnsAsync(Result.Failure("An error occurred while changing the task."));
    
    _mapper
      .Setup(x => x.Map<TaskAggregateState>(It.IsAny<ChangeTaskCommand>()))
      .Returns((TaskAggregateState)new TaskAggregate().State);
    
    // Act
    var result = await _changeTaskCommandHandler.Handle(changeTaskCommand, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsFailure);
    Assert.IsType<Result<TaskResponse>>(result);
    
    _taskRepository.Verify(x => x.LoadByIdAsync(It.IsAny<Guid>()), Times.Once);
    _taskRepository.Verify(x => x.SaveAsync(It.IsAny<TaskAggregate>()), Times.Once);
  }
}
