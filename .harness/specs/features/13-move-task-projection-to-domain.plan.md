# Plan: 13 — Move TaskProjection to Tasks.Domain

**Spec:** `.harness/specs/features/13-move-task-projection-to-domain.md`
**Status:** Done
**Created:** 2026-05-28

## Tasks

- [x] **task-1** · agent: `engineer`
  Move `TaskProjection` to `src/Tasks.Domain/Projections/TaskProjection.cs` (namespace `Tasks.Domain.Projections`), remove the `<ProjectReference>` to `Tasks.Persistence` from `Tasks.Application.csproj`, and update all `using Tasks.Persistence.Reading.Projections` directives in Application and Persistence layers.
  _depends on: —_
  _covers: AC-1, AC-2, AC-3, AC-4, AC-5_
