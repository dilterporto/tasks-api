namespace Tasks.Abstractions.EventSourcing;

public class Event
{
  public Guid Id { get; set; }
  public Guid AggregateId { get; set; }
  public long Version { get; set; }
  public DateTime CreatedAt { get; set; }
  public string? Type { get; set; }
  public string? Data { get; set; }
}
