using Microsoft.Extensions.Logging;
using Tasks.Abstractions.EventSourcing;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Persistence.Reading.Projections.EventCommitters;

public class TaskChangedEventCommitter(
  ProjectionsDbContext projectionsDbContext,
  ILogger<TaskChangedEventCommitter> logger)
  : IEventCommitter<TaskChangedEvent>
{
  public async Task CommitAsync(TaskChangedEvent @event)
  {
    var taskProjection = await projectionsDbContext.Set<TaskProjection>().FindAsync(@event.AggregateId);
    if (taskProjection == null)
    {
      logger.LogError("[Persistence] Task with id {Id} not found", @event.AggregateId);
      return;
    }
    
    taskProjection.Description = @event.Description!;
    taskProjection.Subject = @event.Subject!;
    taskProjection.UserId = @event.UserId;

    await projectionsDbContext.SaveChangesAsync();
    logger.LogInformation("[Persistence] Task projection with id {Id} updated {Projection}", @event.AggregateId, taskProjection);
  }
}
