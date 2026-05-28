# Guide: Domain Events

This guide covers defining, emitting, and handling domain events in the Event Sourcing pattern.

## What is a domain event

A domain event records that something happened in the domain. It is the source of truth for aggregate state — the aggregate is reconstructed by replaying events in order.

Every state change in the `TaskAggregate` must be captured as a domain event.

---

## Event lifecycle

```
Aggregate method called
  → ApplyChange(new XxxEvent { ... })      ← records event in UncommittedChanges
      → Apply(XxxEvent @event)             ← mutates State
          → ITaskRepository.SaveAsync
              → EventsDbContext.Add(events) ← persists to event store
              → IEventCommiters.CommitAllAsync(events)
                  → IEventCommitter<XxxEvent>.CommitAsync ← updates read model
```

---

## Defining a new event

### 1. Create the event class

`src/Tasks.Domain/Aggregates/Tasks/<Name>Event.cs`

```csharp
public class <Name>Event : DomainEvent
{
  // Properties specific to this event
  public string SomeField { get; set; } = default!;
}
```

`DomainEvent` (from `Tasks.Abstractions.Domain`) provides `AggregateId`, `OccurredAt`, and `Version`.

### 2. Emit from the aggregate

In the relevant partial class (e.g., `TaskAggregate.Start.cs`):

```csharp
public void <Method>(<params>)
{
  ApplyChange(new <Name>Event
  {
    Field = value
  });
}

public void Apply(<Name>Event @event)
{
  this.State.SomeField = @event.SomeField;
  // update all state fields affected by this event
}
```

Rules:
- `ApplyChange` records the event and immediately calls `Apply`
- `Apply` is also called during event replay (aggregate reconstruction) — it must be a pure state mutation
- Never call `Apply` directly — always go through `ApplyChange`

### 3. Create the event committer

`src/Tasks.Persistence/Reading/Projections/EventCommitters/<Name>EventCommitter.cs`

```csharp
public class <Name>EventCommitter(ProjectionsDbContext context)
  : IEventCommitter<<Name>Event>
{
  public async Task CommitAsync(<Name>Event @event)
  {
    var projection = await context.Tasks.FindAsync(@event.AggregateId);
    if (projection is null) return;

    projection.SomeField = @event.SomeField;
    await context.SaveChangesAsync();
  }
}
```

### 4. Register the committer

`src/Tasks.Persistence/DependencyExtensions.cs` — add:

```csharp
services.AddScoped<IEventCommitter<<Name>Event>, <Name>EventCommitter>();
```

---

## Event store

Events are stored in the `events` table via `EventsDbContext`. Each row includes:
- `AggregateId` — the aggregate's identity
- `EventType` — discriminator for deserialization
- `Payload` — JSON-serialized event body
- `OccurredAt` / `Version` — ordering metadata

The `EventsDbContext` is append-only. Events are never updated or deleted.

---

## Aggregate reconstruction

`ITaskRepository.LoadByIdAsync` reads all events for an aggregate from `EventsDbContext`, ordered by `Version`, and replays them:

```
new TaskAggregate(id) → foreach event → Apply(event) → fully reconstituted aggregate
```
