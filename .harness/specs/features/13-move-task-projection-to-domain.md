# Spec: Move TaskProjection to Tasks.Domain

**Issue:** #13  
**Author:** @dilterporto  
**Status:** Implementing  
**Agent:** engineer  

---

## Context

`TaskProjection` is the read model DTO used by query handlers in `Tasks.Application`. It lives in `Tasks.Persistence.Reading.Projections`, which forces `Tasks.Application` to hold a direct `<ProjectReference>` to `Tasks.Persistence`. This violates the Clean Architecture rule that Application must only depend on Domain and Abstractions — not on Infrastructure.

Without this fix, the architecture sensor CI job (issue #15) cannot be enabled: the baseline would fail on day one.

---

## Specification

### What will be built

`TaskProjection` is relocated to `Tasks.Domain`. All consumers update their `using` directives. No behavior changes — only namespace and project membership change.

### Inputs and outputs

No change to API contracts or handler signatures. The type `TaskProjection` remains identical; only its namespace changes from `Tasks.Persistence.Reading.Projections` to `Tasks.Domain.Projections`.

### Behavior

- **Must** move `TaskProjection` to `src/Tasks.Domain/Projections/TaskProjection.cs`
- **Must** use namespace `Tasks.Domain.Projections`
- **Must** remove `<ProjectReference Include="..\Tasks.Persistence\Tasks.Persistence.csproj" />` from `Tasks.Application.csproj`
- **Must** update all `using Tasks.Persistence.Reading.Projections` in `Tasks.Application/` (3 files: `TaskProfile.cs`, `GetUpcomingTasksQuery.Handler.cs`, `GetTaskByIdQuery.Handler.cs`)
- **Must** update `using` in Persistence event committers that reference `TaskProjection`
- **Must not** change any property on `TaskProjection`
- **Must not** change `IProjectionsReader<T>` — it stays in `Tasks.Abstractions`

---

## Acceptance Criteria

- [ ] AC-1: `Tasks.Application.csproj` has no `<ProjectReference>` to `Tasks.Persistence`
- [ ] AC-2: `TaskProjection` is in namespace `Tasks.Domain.Projections`
- [ ] AC-3: No `using Tasks.Persistence` in `src/Tasks.Application/`
- [ ] AC-4: `dotnet build Tasks.sln` passes with no errors
- [ ] AC-5: `dotnet test tests/Tasks.Tests/` passes with no failures

---

## Technical Constraints

- Layers affected: `Tasks.Domain`, `Tasks.Application`, `Tasks.Persistence`
- `Tasks.Domain` must not gain any new package dependency as a result of this move
- `TaskProjection` extends `Projection` (from `Tasks.Abstractions.EventSourcing`) — this dependency is valid since `Tasks.Domain` already references `Tasks.Abstractions`

---

## Out of Scope

- Changing `IProjectionsReader<T>` location
- Changing `ProjectionsDbContext` EF Core configuration
- Any behavior change in query handlers

---

## Sensors

- [ ] Architecture: `grep -r "Tasks.Persistence" src/Tasks.Application/ --include="*.csproj"` → empty
- [ ] Architecture: `grep -r "using Tasks.Persistence" src/Tasks.Application/ --include="*.cs"` → empty
- [ ] Build: `dotnet build Tasks.sln` → exit 0
- [ ] Test: `dotnet test tests/Tasks.Tests/` → exit 0
