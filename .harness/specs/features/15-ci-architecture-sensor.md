# Spec: Add architecture-sensor job to CI

**Issue:** #15  
**Author:** @dilterporto  
**Status:** Implementing  
**Agent:** engineer  

---

## Context

Architecture fitness checks in `.harness/sensors/architecture-fitness.md` are currently manual. There is no automated gate preventing a PR from introducing a layer dependency violation. Issues #13 and #14 fix the current baseline violations; this issue adds the CI enforcement so the rules are checked automatically on every PR.

---

## Specification

### What will be built

A new `architecture-sensor` job in `.github/workflows/ci.yml` that runs the grep checks from `architecture-fitness.md` in parallel with `build-and-test`. The job becomes a required status check so violations block merge.

### Inputs and outputs

```
Input:  source tree at HEAD
Output: CI job pass (all checks green) or fail (at least one violation found, with descriptive message)
```

### Behavior

- **Must** add a new job `architecture-sensor` to `.github/workflows/ci.yml`
- **Must** run in parallel with `build-and-test` (no `needs:` dependency between them)
- **Must** check all 7 rules from `architecture-fitness.md`:
  1. `Application` must not reference `Persistence` in `.csproj`
  2. `Domain` must not reference other layers in `.csproj`
  3. `Api` must not reference `Persistence` in `.csproj`
  4. `Application` must not use `ProjectionsDbContext` in `.cs`
  5. `Application` must not use `EventsDbContext` in `.cs`
  6. `Application` must not import `using Tasks.Persistence` in `.cs`
  7. All `*Handler.cs` files in `Application/UseCases/` must contain `Result`
- **Must** print a descriptive failure message for each violated rule
- **Must** exit non-zero on any violation
- **Must** not require .NET SDK (pure shell, `ubuntu-latest` runner)
- **Must** update `.harness/sensors/architecture-fitness.md` to reference the CI job and remove "future" language
- **Should** add `architecture-sensor` as a required status check in branch protection (manual step via GitHub UI or `gh` CLI)

---

## Acceptance Criteria

- [ ] AC-1: `architecture-sensor` job exists in `ci.yml` and runs on every PR to `main`
- [ ] AC-2: job passes on `main` after #13 and #14 are merged
- [ ] AC-3: introducing a forbidden `using Tasks.Persistence` in Application causes the job to fail
- [ ] AC-4: `architecture-fitness.md` no longer says "Automated enforcement (future)"
- [ ] AC-5: `architecture-sensor` is a required status check (branch protection)

---

## Technical Constraints

- Layers affected: `.github/workflows/ci.yml`, `.harness/sensors/architecture-fitness.md`
- **Must** be implemented after #13 and #14 are merged — the clean baseline is required
- No new NuGet packages
- No new GitHub Actions marketplace actions

---

## Out of Scope

- ArchUnitNET integration
- Sensor checks for `Tasks.DependencyInjection` or `Tasks.Api` internal rules
- Automated branch protection update (done manually after job is added)

---

## Sensors

- [ ] CI: `architecture-sensor` job runs and passes on `main`
- [ ] CI: `architecture-sensor` job fails when a violation is manually introduced in a test branch
- [ ] Docs: `architecture-fitness.md` references `.github/workflows/ci.yml`
