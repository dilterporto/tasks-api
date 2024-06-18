using MediatR;

namespace Tasks.Abstractions.Domain;

public interface IDomainEvent : INotification
{
  Guid Id { get; set; }
  Guid AggregateId { get; set; }
  long Version { get; set; }
  DateTime CreatedAt { get; set; }
}
