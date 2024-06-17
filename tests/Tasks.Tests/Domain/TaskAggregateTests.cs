using AutoFixture;
using Tasks.Domain.Aggregates.Tasks;
using Xunit;
using TaskStatus = Tasks.Domain.Aggregates.Tasks.TaskStatus;

namespace Tasks.Tests.Domain;

public class TaskAggregateTests 
{
  [Fact]
  public void Delete_WithValidReason_ShouldDeleteTask()
  {
    // Arrange
    var task = new TaskAggregate();
    var reason = new Fixture().Create<string>();

    // Act
    task.Delete(reason);

    // Assert
    Assert.Single(task.UncommittedEvents);
    Assert.IsType<TaskDeletedEvent>(task.UncommittedEvents.First());
    Assert.True(task.Removed);
  }
  
  [Fact]
  public void Change_WithValidState_ShouldChangeTask()
  {
    // Arrange
    var task = new TaskAggregate();
    var state = new Fixture().Create<TaskAggregateState>();

    // Act
    task.Change(state);

    // Assert
    Assert.Single(task.UncommittedEvents);
    Assert.IsType<TaskChangedEvent>(task.UncommittedEvents.First());
    Assert.Equal(state.Subject, task.State.Subject);
    Assert.Equal(state.Description, task.State.Description);
    Assert.Equal(state.UserId, task.State.UserId);
  }
  
  [Fact]
  public void Apply_WithTaskCreatedEvent_ShouldSetState()
  {
    // Arrange
    var task = new TaskAggregate();
    var @event = new Fixture().Create<TaskCreatedEvent>();

    // Act
    task.Apply(@event);

    // Assert
    Assert.Equal(@event.AggregateId, task.State.Id);
    Assert.Equal(@event.UserId, task.State.UserId);
    Assert.Equal(@event.At, task.State.At);
    Assert.Equal(@event.Subject, task.State.Subject);
    Assert.Equal(@event.Description, task.State.Description);
    Assert.Equal(TaskStatus.Created, task.State.Status);
  }
  
  [Fact]
  public void Apply_WithTaskChangedEvent_ShouldSetState()
  {
    // Arrange
    var task = new TaskAggregate();
    var @event = new Fixture().Create<TaskChangedEvent>();

    // Act
    task.Apply(@event);

    // Assert
    Assert.Equal(@event.Subject, task.State.Subject);
    Assert.Equal(@event.Description, task.State.Description);
    Assert.Equal(@event.UserId, task.State.UserId);
  }
  
  [Fact]
  public void Apply_WithTaskDeletedEvent_ShouldMarkAsRemoved()
  {
    // Arrange
    var task = new TaskAggregate();
    var @event = new Fixture().Create<TaskDeletedEvent>();

    // Act
    task.Apply(@event);

    // Assert
    Assert.True(task.Removed);
  }
  
  [Fact]
  public void MarkChangesAsCommitted_ShouldClearUncommittedEvents()
  {
    // Arrange
    var task = new TaskAggregate();
    task.UncommittedEvents.Add(new Fixture().Create<TaskCreatedEvent>());

    // Act
    task.MarkChangesAsCommitted();

    // Assert
    Assert.Empty(task.UncommittedEvents);
  }
}
