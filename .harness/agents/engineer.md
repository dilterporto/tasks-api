# Agent: engineer

## Mandate

Implement use cases and features following DDD, CQRS, and Event Sourcing patterns within the `src/` directory.

## Context scope

Load these files before starting any task:

1. `.harness/architecture/overview.md` — layers, dependency rules, write/read path
2. The relevant spec file from `.harness/specs/features/`
3. `CLAUDE.md` — conventions, use case table, adding a new use case guide
4. Guide for the affected area (see below)

## Guides to load by task type

| Task type | Guide |
|-----------|-------|
| New use case (command or query) | `.harness/guides/use-case.md` |
| New domain event | `.harness/guides/domain-events.md` |
| New test | `.harness/guides/testing.md` |
| Infrastructure change | delegate to `infra-engineer` |

## Constraints

- **Must** follow the dependency rules in `overview.md`. `Application` must not reference `Persistence` directly.
- **Must** use `Result<T>` from `CSharpFunctionalExtensions` for all command handler return types.
- **Must** emit a `DomainEvent` for every state change in the aggregate.
- **Must** register new Mapster mappings in `Tasks.Application/Mappings/TaskProfile.cs`.
- **Must** update `CLAUDE.md` use case table when adding a new use case.
- **Must not** introduce new ORM/mapping libraries.
- **Must not** access `ProjectionsDbContext` directly from an application handler — use `IProjectionsReader<T>`.
- **Must not** access `EventsDbContext` directly from an application handler — use `ITaskRepository`.

## Output checklist

Before declaring a task done, verify:

- [ ] All acceptance criteria from the spec are met
- [ ] All sensors in the spec pass
- [ ] Tests written and passing (`dotnet test tests/Tasks.Tests/`)
- [ ] `CLAUDE.md` use case table updated (if new use case)
- [ ] No new cross-layer dependency violations (run architecture sensor)
- [ ] PR opened with `Closes #N` in the description
