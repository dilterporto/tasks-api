using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tasks.Abstractions;
using Tasks.Abstractions.EventSourcing;
using Tasks.Persistence.Events;

namespace Tasks.Persistence;

public class Repository<TAggregate, TRepository>(EventsDbContext dbContext, IEventCommiters eventCommiters, ILogger<TRepository> logger)
  where TAggregate : AggregateRoot
{
  public async Task<Maybe<TAggregate>> LoadByIdAsync(Guid id)
  {
    var events = await dbContext.Set<Event>()
      .Where(e => e.AggregateId == id)
      .OrderBy(e => e.Version)
      .ToListAsync();

    if (events.Count == 0)
    {
      return Maybe<TAggregate>.None;
    }

    var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    aggregate.Id = id;

    foreach (DomainEvent? eventData in events.Select(@event => JsonConvert
               .DeserializeObject(@event.Data!, Type.GetType(@event.Type!)!)))
      aggregate.ApplyChange(eventData!, isNew: false);

    return aggregate;
  }

  public async Task SaveAsync(TAggregate aggregate)
  {
    PersistAggregateEvents(aggregate);

    await CommitEvents(aggregate);
    
    aggregate.MarkChangesAsCommitted();
  }

  private async Task CommitEvents(TAggregate aggregate) 
    => await eventCommiters.CommitAllAsync(aggregate.UncommittedEvents);

  private void PersistAggregateEvents(TAggregate aggregate)
  {
    var events = aggregate.UncommittedEvents
      .Select(@event => new Event
      {
        Id = @event.Id,
        AggregateId = aggregate.Id,
        Version = @event.Version,
        CreatedAt = @event.CreatedAt,
        Type = $"{@event.GetType().FullName}, {@event.GetType().Assembly.GetName().Name}",
        Data = JsonConvert.SerializeObject(@event)
      });

    foreach (var @event in events)
    {
      logger.LogInformation("[Persistence] Saving {EventType} {@Event}", @event.GetType(), @event);
      dbContext.Add(@event);
    }
      
  }
}
