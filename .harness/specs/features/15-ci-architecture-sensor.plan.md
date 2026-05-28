# Plan: 15 — Add architecture-sensor job to CI

**Spec:** `.harness/specs/features/15-ci-architecture-sensor.md`
**Status:** In Progress
**Created:** 2026-05-28

## Tasks

- [x] **task-1** · agent: `engineer`
  Add the `architecture-sensor` job to `.github/workflows/ci.yml` with all 7 shell grep checks running in parallel with `build-and-test`, and update `.harness/sensors/architecture-fitness.md` to reference the CI job and remove "future" language.
  _depends on: —_
  _covers: AC-1, AC-2, AC-3, AC-4_

- [ ] **task-2** · agent: `engineer`
  Enable `architecture-sensor` as a required status check in branch protection via `gh` CLI (`gh api` PATCH on branch protection rules).
  _depends on: task-1_
  _covers: AC-5_
