using Microsoft.Extensions.Logging;
using Tasks.Abstractions.EventSourcing;
using Tasks.Domain.Aggregates.Tasks;
using TaskStatus = Tasks.Domain.Aggregates.Tasks.TaskStatus;

namespace Tasks.Persistence.Reading.Projections.EventCommitters;

public class TaskStartedEventCommitter(
  ProjectionsDbContext projectionsDbContext,
  ILogger<TaskChangedEventCommitter> logger) : IEventCommitter<TaskStartedEvent>
{
  public async Task CommitAsync(TaskStartedEvent @event)
  { 
    var taskProjection = await projectionsDbContext.Set<TaskProjection>().FindAsync(@event.AggregateId);
    if (taskProjection == null)
    {
      logger.LogError("[Persistence] Task with id {Id} not found", @event.AggregateId);
      return;
    }
    
    taskProjection.StartedAt = @event.StartedAt;
    taskProjection.UserId = @event.AssignedTo;
    taskProjection.Status = $"{TaskStatus.Started}";

    await projectionsDbContext.SaveChangesAsync();
    logger.LogInformation("[Persistence] Task projection with id {Id} updated {Projection}", @event.AggregateId, taskProjection);
  }
}
