using Microsoft.Extensions.Logging;
using Tasks.Abstractions.EventSourcing;
using Tasks.Domain.Aggregates.Tasks;
using TaskStatus = Tasks.Domain.Aggregates.Tasks.TaskStatus;

namespace Tasks.Persistence.Reading.Projections.EventCommitters;

public class TaskCreatedEventCommitter(ProjectionsDbContext projectionsDbContext, ILogger<TaskCreatedEventCommitter> logger) : IEventCommitter<TaskCreatedEvent>
{
  public async Task CommitAsync(TaskCreatedEvent @event)
  {
    var taskProjection = new TaskProjection
    {
      Id = @event.AggregateId,
      Description = @event.Description,
      Subject = @event.Subject,
      Status = $"{TaskStatus.Created}",
      At = @event.At,
      DueAt = @event.DueAt,
    };
    
    projectionsDbContext.Add(taskProjection);

    await projectionsDbContext.SaveChangesAsync();
    logger.LogInformation("[Persistence] Task projection with id {Id} created {Projection}", @event.AggregateId, taskProjection);
  }
}
