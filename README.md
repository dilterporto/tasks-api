# Tasks API

Task management API built with .NET 8, implementing CQRS, Event Sourcing, and Domain-Driven Design.

## Architecture

Clean Architecture with DDD, separating concerns across four layers:

```
┌──────────────────────────────────────────────┐
│         Presentation (Tasks.Api)             │
│         REST API via FastEndpoints           │
├──────────────────────────────────────────────┤
│        Application (Tasks.Application)       │
│        Commands, Queries, Handlers           │
├──────────────────────────────────────────────┤
│          Domain (Tasks.Domain)               │
│       Aggregates, Events, Repositories       │
├──────────────────────────────────────────────┤
│       Infrastructure (Tasks.Persistence)     │
│    Event Store, Projections, Repositories    │
└──────────────────────────────────────────────┘
```

### Architectural Patterns

- **CQRS** — Commands (write) and Queries (read) handled independently via MediatR
- **Event Sourcing** — State changes persisted as domain events; aggregates reconstructed from event history
- **Read Model** — Dedicated projection table (`tasks`) updated by event committers, optimized for queries
- **Result Pattern** — `CSharpFunctionalExtensions.Result<T>` for explicit error handling
- **Pipeline Behaviors** — Cross-cutting concerns (logging, unit of work, cache invalidation) as MediatR decorators
- **Cache-Aside** — Redis caches upcoming tasks; invalidated after any mutating command

## Project Structure

```
src/
├── Tasks.Api/                  # FastEndpoints, validation, request/response mapping
├── Tasks.Application/          # Use cases as Commands and Queries
│   ├── UseCases/               # CreateTask, ChangeTask, DeleteTask, GetTaskById, GetUpcomingTasks
│   ├── Behaviors/              # UnitOfWork, Logging, CacheValidation pipeline behaviors
│   └── Contracts/              # Response DTOs (TaskResponse, UpcomingTasksResponse)
├── Tasks.Domain/               # TaskAggregate, domain events, ITaskRepository
├── Tasks.Persistence/          # EF Core DbContexts, repositories, event committers, projections
├── Tasks.Abstractions/         # Shared interfaces: ICacheManager, IEventCommitter, AggregateRoot
└── Tasks.DependencyInjection/  # Centralized DI configuration

tests/
└── Tasks.Tests/                # Unit tests for domain and application layers
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/tasks` | Create a task |
| `GET` | `/api/tasks/{id}` | Get task by ID |
| `PUT` | `/api/tasks/{id}` | Update task subject, description, or user |
| `DELETE` | `/api/tasks/{id}` | Delete a task |
| `GET` | `/api/tasks/upcoming` | Get upcoming tasks grouped by date (cached) |

### Create Task — `POST /api/tasks`

```json
{
  "userId": "guid",
  "subject": "string",
  "description": "string (max 500 chars)",
  "dueAt": "datetime (must be in the future)"
}
```

### Update Task — `PUT /api/tasks/{id}`

```json
{
  "userId": "guid",
  "subject": "string",
  "description": "string (max 500 chars)"
}
```

## Task Lifecycle

```
Created → Started → Completed
```

Domain events emitted at each transition:

| Event | Trigger |
|-------|---------|
| `TaskCreatedEvent` | Task creation |
| `TaskStartedEvent` | Task started |
| `TaskChangedEvent` | Subject/description/user updated |
| `TaskDeletedEvent` | Task deleted |

## Event Sourcing

Every state change is stored as an event in the `events` table. On reads, the `TaskAggregate` is reconstructed by replaying events in order. The `tasks` projection table is kept in sync by event committers after each write.

```
Command → Aggregate (apply event) → EventsDbContext (event store)
                                  → EventCommitter  → ProjectionsDbContext (read model)
```

## Technology Stack

| Concern | Technology |
|---------|------------|
| Framework | .NET 8, FastEndpoints |
| CQRS | MediatR |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL |
| Migrations | Flyway |
| Caching | Redis Stack |
| Logging | Serilog + Seq |
| Object mapping | Mapster |
| Testing | xUnit, Moq, AutoFixture |

## Running Locally

**Requirements:** Git and Docker.

```bash
git clone <repository-url>
cd backend-challenge
docker-compose up
```

### Services

| Service | URL |
|---------|-----|
| API + Swagger | http://localhost:5006/swagger |
| Seq (logs) | http://localhost:5341 |
| RedisInsight | http://localhost:8001 |

### Development environment (infrastructure only)

```bash
docker-compose -f docker-compose.dev-env.yml up
dotnet run --project src/Tasks.Api
```

## Running Tests

```bash
dotnet test tests/Tasks.Tests/
```
