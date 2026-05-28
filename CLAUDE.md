# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build Tasks.sln

# Run all tests
dotnet test tests/Tasks.Tests/

# Run a single test class
dotnet test tests/Tasks.Tests/ --filter "FullyQualifiedName~CreateTasksCommandTests"

# Run a single test method
dotnet test tests/Tasks.Tests/ --filter "FullyQualifiedName~CreateTasksCommand_ShouldCreateWithSuccess"

# Run the API (requires infrastructure running)
dotnet run --project src/Tasks.Api

# Start full environment (API + infra)
docker-compose up

# Start infrastructure only (run API locally)
docker-compose -f docker-compose.dev-env.yml up
```

## Infra

```bash
# Start LocalStack (required before terraform commands against local env)
docker-compose -f docker-compose.dev-env.yml up localstack

# Local environment (LocalStack)
cd infra/environments/local
terraform init
terraform plan
terraform apply

# Production environment (requires AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_REGION)
cd infra/environments/prod
terraform init   # initializes S3 remote state backend
terraform plan
terraform apply
```

## Architecture

Clean Architecture with CQRS and Event Sourcing across five projects:

- **Tasks.Abstractions** — Shared interfaces and base classes (`AggregateRoot`, `ICacheManager`, `IEventCommitter<T>`, `IEventsCommiter`, pipeline behavior base classes). Depends on `Mapster` for the object mapping abstraction.
- **Tasks.Domain** — `TaskAggregate` (the only aggregate), four domain events, `ITaskRepository`. Pure domain logic.
- **Tasks.Application** — MediatR commands/queries in `UseCases/`. Three pipeline behaviors in `Behaviors/`: `UnitOfWorkPipelineBehavior`, `LoggingBehavior`, `CacheValidationPipelineBehavior`.
- **Tasks.Persistence** — Two EF Core DbContexts: `EventsDbContext` (event store, table `events`) and `ProjectionsDbContext` (read model, table `tasks`). `Repository<T>` reconstructs aggregates by replaying events. `EventCommitters` dispatches to four typed `IEventCommitter<T>` implementations that update projections after each event.
- **Tasks.Api** — FastEndpoints. One endpoint per use case. FluentValidation via FastEndpoints validator classes.
- **Tasks.DependencyInjection** — Single `AddDependencies()` extension method; wires everything together.

## CQRS Flow

**Write path:**
1. FastEndpoints receives request → maps to Command → sends via MediatR
2. `UnitOfWorkPipelineBehavior` wraps the handler in a transaction (calls `EventsDbContext.SaveChangesAsync` after)
3. Handler loads aggregate via `ITaskRepository.LoadByIdAsync` → calls aggregate method → saves via `SaveAsync`
4. `Repository.SaveAsync` persists new events to `EventsDbContext`, then calls `EventCommitters.CommitAllAsync`
5. `EventCommitters` dispatches each event to the matching `IEventCommitter<T>`, which updates `ProjectionsDbContext`
6. `CacheValidationPipelineBehavior` invalidates Redis cache after any command

**Read path:**
1. Query handler calls `IProjectionsReader<TaskProjection>` directly against `ProjectionsDbContext`
2. `GetUpcomingTasksQuery` checks Redis cache first (`UpcomingTasksKey`); on miss, queries DB and caches result

## Use Cases

| Use Case | Type | Handler |
|----------|------|---------|
| Create task | Command | `CreateTaskCommandHandler` |
| Change task (subject/description/user) | Command | `ChangeTaskCommandHandler` |
| Delete task | Command | `DeleteTaskCommandHandler` |
| Get task by ID | Query | `GetTaskByIdQueryHandler` |
| Get upcoming tasks (grouped by date) | Query | `GetUpcomingTasksQueryHandler` |

## Domain Events

`TaskCreatedEvent`, `TaskStartedEvent`, `TaskChangedEvent`, `TaskDeletedEvent` — all extend `DomainEvent`. Events are stored serialized as JSON in the `events.data` column (type `jsonb`). The type assembly-qualified name is stored in `events.type` for deserialization during aggregate replay.

## Adding a New Use Case

1. Add a command/query record in `Tasks.Application/UseCases/<Name>/`
2. Add a handler implementing `IRequestHandler<TCommand, Result<TResponse>>`
3. If the use case mutates state: add a domain method on `TaskAggregate` that calls `ApplyChange(new SomeDomainEvent(...))`
4. If a new event type is introduced: add `IEventCommitter<TNewEvent>` implementation in `Tasks.Persistence/Reading/Projections/EventCommitters/`, register it in `DependencyExtensions.cs` and in `EventCommitters` constructor
5. If new object mappings are needed: add `config.NewConfig<TSource, TDest>()` in `TaskProfile.cs` (`Tasks.Application/Mappings/`) or `TaskMappings.cs` (`Tasks.Api/Apis/Tasks/Mappings/`) depending on the layer
6. Register the handler (MediatR scans assemblies automatically; no manual registration needed)
7. Add a FastEndpoints endpoint in `Tasks.Api/Apis/Tasks/`
8. Update the use case table in `CLAUDE.md` and the endpoints table in `README.md`

## Git and Issue Workflow

This project follows **trunk-based development**: short-lived branches merged into `main` frequently via squash merge. `main` is always deployable.

**Branch naming:** `feat/#<issue>-short-description`, `fix/#<issue>-short-description`, `chore/#<issue>-short-description`

Branches should live no longer than 1–2 days. If a feature takes longer, use a feature flag or merge an incomplete-but-safe implementation behind a condition.

**Every PR must:**
1. Reference an open issue: `Closes #<N>` in the body (auto-closes issue and moves Project card to Done on merge)
2. Pass CI (build + tests) before merge
3. Use squash merge — one commit per issue on `main`

PR body template (`.github/pull_request_template.md` pre-fills this):
```
## Summary
- What changed and why

Closes #N
```

**Never close issues manually** via the GitHub Project board — only PR merges close issues.

## CI/CD

**CI** (`.github/workflows/ci.yml`) — runs on every push to `main` and on every PR:
- `dotnet restore` → `dotnet build` → `dotnet test`
- Must pass before merge (enforced by branch protection)

**CD** (`.github/workflows/cd.yml`) — runs on every push to `main`:
- Publishes the app, builds Docker image, pushes to Amazon ECR
- ECS deploy step is scaffolded but requires AWS secrets configured:
  - `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`
  - `ECR_REPOSITORY`, `ECS_CLUSTER`, `ECS_SERVICE`

## Key Conventions

- Commands return `Result<TaskResponse>` via `CSharpFunctionalExtensions`. Use `.IsSuccess`/`.IsFailure`, chain with `.Tap`/`.Map`/`.Finally`.
- Aggregates are never `new`-ed directly in handlers — always loaded via repository or created through the aggregate constructor (which emits `TaskCreatedEvent`).
- Tests use constructor injection of mocks; no test base classes. AutoFixture generates test data; Moq for dependencies.
- Package versions are centrally managed in `Directory.Packages.props` — add version there, reference without version in `.csproj`.
- Object mapping uses **Mapster** (`IRegister` / `TypeAdapterConfig`). Define mappings in `TaskProfile` (Application layer) or `TaskMappings` (Api layer). Inject `MapsterMapper.IMapper` in handlers and endpoints.

## Harness Engineering

This project uses Spec-Driven Development via the `.harness/` directory:

```
.harness/
├── agents/          ← Specialized agent mandates (engineer, architect, reviewer, infra-engineer)
├── architecture/    ← Architecture overview and ADRs
│   └── adr/
├── guides/          ← How-to guides for recurring patterns
├── sensors/         ← Fitness checks for architecture and spec compliance
└── specs/           ← Feature specs (one file per issue)
    └── features/
```

**Workflow for new features:**
1. `architect` writes a spec in `.harness/specs/features/<N>-<slug>.md`
2. `planner` reads the spec and writes `.harness/specs/features/<N>-<slug>.plan.md` — decomposes into tasks, each assigned to an agent (`engineer`, `infra-engineer`, `architect`)
3. Implementation agents execute their assigned tasks from the plan
4. `reviewer` checks against spec sensors before merge

**Starting a task with an agent:**
```
Agent: engineer
Task: implement issue #N
Spec: .harness/specs/features/N-slug.md
Plan: .harness/specs/features/N-slug.plan.md
```

See `.harness/agents/AGENTS.md` for the full agent registry and spec-first rule.
