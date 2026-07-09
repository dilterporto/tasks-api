# Architecture Overview

## System Description

Tasks API is a task management backend built on .NET 8 following Clean Architecture, Domain-Driven Design, CQRS, and Event Sourcing.

## Layers

```
┌──────────────────────────────────────────────┐
│         Presentation (Tasks.Api)             │
│         REST API via FastEndpoints           │
├──────────────────────────────────────────────┤
│        Application (Tasks.Application)       │
│        Commands, Queries, Pipeline Behaviors │
├──────────────────────────────────────────────┤
│          Domain (Tasks.Domain)               │
│       Aggregates, Events, Repositories       │
├──────────────────────────────────────────────┤
│       Infrastructure (Tasks.Persistence)     │
│    Event Store, Projections, Repositories    │
└──────────────────────────────────────────────┘
       Tasks.Abstractions (cross-cutting)
       Tasks.DependencyInjection (wiring)
```

### Dependency rules

- `Domain` has no dependencies on other project layers
- `Application` depends on `Domain` and `Abstractions`
- `Persistence` depends on `Domain` and `Abstractions`
- `Api` depends on `Application` and `DependencyInjection`
- `DependencyInjection` depends on `Persistence`

Violations of these rules are architecture sensor failures.

## Write Path (Commands)

```
HTTP Request
  → FastEndpoints (Tasks.Api)
  → IMapper.Map → Command
  → MediatR.Send
      → UnitOfWorkPipelineBehavior (opens transaction)
      → LoggingBehavior
      → CacheValidationPipelineBehavior (invalidates Redis on success)
      → CommandHandler (Tasks.Application)
          → ITaskRepository.LoadByIdAsync (replays events from EventsDbContext)
          → TaskAggregate.<method>() → ApplyChange(new DomainEvent)
          → ITaskRepository.SaveAsync
              → EventsDbContext.Add(events)  ← event store
              → EventCommitters.CommitAllAsync
                  → IEventCommitter<T>.CommitAsync  ← updates ProjectionsDbContext
```

## Read Path (Queries)

```
HTTP Request
  → FastEndpoints (Tasks.Api)
  → MediatR.Send
      → QueryHandler (Tasks.Application)
          → IProjectionsReader<TaskProjection>.GetByIdAsync / GetAllAsync
              → ProjectionsDbContext  ← read model (table: tasks)
          → IMapper.Map → Response DTO
```

`GetUpcomingTasksQuery` additionally checks Redis cache before hitting the DB.

## Data Stores

| Store | Technology | Purpose |
|-------|-----------|---------|
| Event store | PostgreSQL (`events` table) | Immutable append-only log of all domain events |
| Read model | PostgreSQL (`tasks` table) | Denormalized projection for fast queries |
| Cache | Redis | Short-lived cache for `GET /api/tasks/upcoming` |

## Domain Model

```
TaskAggregate (AggregateRoot)
  State: TaskAggregateState
  Status: Created → Started → Completed

Domain Events:
  TaskCreatedEvent   → emitted on new TaskAggregate(state)
  TaskStartedEvent   → emitted on TaskAggregate.Start(userId)
  TaskChangedEvent   → emitted on TaskAggregate.Change(state)
  TaskDeletedEvent   → emitted on TaskAggregate.Delete(reason)
```

## Infrastructure (planned)

See `ADR-005-terraform-aws-infrastructure.md` and issue #10.

```
Internet → ALB (public subnets, port 80)
         → ECS Fargate (Tasks.Api container, private subnets)
         → RDS PostgreSQL (private subnet)
         → ElastiCache Redis (private subnet)
```

## Architecture Decision Records

| ADR | Decision | Status |
|-----|---------|--------|
| [ADR-001](adr/ADR-001-cqrs-mediatr.md) | CQRS via MediatR | Accepted |
| [ADR-002](adr/ADR-002-event-sourcing-postgresql.md) | Event Sourcing with custom PostgreSQL store | Accepted |
| [ADR-003](adr/ADR-003-mapster-over-automapper.md) | Mapster replaces AutoMapper | Accepted |
| [ADR-004](adr/ADR-004-trunk-based-development.md) | Trunk-based development | Accepted |
| [ADR-005](adr/ADR-005-terraform-aws-infrastructure.md) | Terraform for AWS infrastructure | Proposed |
| [ADR-006](adr/ADR-006-domain-event-projection-via-mediatr.md) | Domain event projection via MediatR INotificationHandler | Proposed |
| [ADR-007](adr/ADR-007-opentelemetry-instrumentation.md) | OpenTelemetry instrumentation (traces, metrics, log correlation) | Proposed |
