using Microsoft.Extensions.Logging;
using Tasks.Abstractions.EventSourcing;
using Tasks.Domain.Aggregates.Tasks;
using Tasks.Persistence.Events;

namespace Tasks.Persistence.Repositories;

public class TaskRepository(EventsDbContext dbContext, IEventCommiters eventCommiters, ILogger<ITaskRepository> logger) 
  : Repository<TaskAggregate, ITaskRepository>(dbContext, eventCommiters, logger), ITaskRepository
{

}
