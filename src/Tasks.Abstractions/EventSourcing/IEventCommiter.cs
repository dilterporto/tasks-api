using Tasks.Abstractions.Domain;

namespace Tasks.Abstractions.EventSourcing;

public interface IEventCommitter<in TEvent> where TEvent : IDomainEvent
{
  Task CommitAsync(TEvent @event);
}

public interface IEventCommiters
{
  Task CommitAllAsync(IEnumerable<IDomainEvent> events);
}
