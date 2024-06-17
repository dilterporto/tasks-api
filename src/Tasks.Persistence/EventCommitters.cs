using Microsoft.Extensions.Logging;
using Tasks.Abstractions;
using Tasks.Abstractions.EventSourcing;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Persistence;

public class EventCommitters : IEventCommiters
{
  private readonly ILogger<EventCommitters> _logger;

  private readonly IDictionary<Type, Func<IDomainEvent, Task>> _eventCommiters
    = new Dictionary<Type, Func<IDomainEvent, Task>>();

  public EventCommitters(
    IEventCommitter<TaskCreatedEvent> taskCreatedEventCommitter,
    IEventCommitter<TaskChangedEvent> taskChangedEventCommitter,
    IEventCommitter<TaskDeletedEvent> taskDeletedEventCommitter,
    ILogger<EventCommitters> logger)
  {
    _logger = logger;
    _eventCommiters.Add(typeof(TaskCreatedEvent), RunAsync(taskCreatedEventCommitter));
    _eventCommiters.Add(typeof(TaskChangedEvent), RunAsync(taskChangedEventCommitter));
    _eventCommiters.Add(typeof(TaskDeletedEvent), RunAsync(taskDeletedEventCommitter));
  }
  
  private static Func<IDomainEvent, Task> RunAsync<TEvent>(IEventCommitter<TEvent> accountEventCommiter)
    where TEvent : IDomainEvent =>
    async (@event) => await accountEventCommiter
      .CommitAsync((TEvent)@event);
  
  public async Task CommitAllAsync(IEnumerable<IDomainEvent> events)
  {
    foreach (var @event in events)
    {
      if (!_eventCommiters.ContainsKey(@event.GetType()))
      {
        _logger.LogWarning("[Persistence] No committer found for {EventType} {@Event}", @event.GetType(), @event);
        continue;
      }

      var committer = _eventCommiters[@event.GetType()];
      await committer.Invoke(@event);
      _logger.LogInformation("[Persistence] Committed {EventType} {@Event}", @event.GetType(), @event);
    }
  }
}
