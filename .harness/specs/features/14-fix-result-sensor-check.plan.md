# Plan: 14 — Fix architecture sensor Result check

**Spec:** `.harness/specs/features/14-fix-result-sensor-check.md`
**Status:** Done
**Created:** 2026-05-28

## Tasks

- [x] **task-1** · agent: `engineer`
  Update the Result pattern grep command in `.harness/sensors/architecture-fitness.md` from `grep -rL "Result<"` to `grep -rL "Result"` so non-generic `Result` (used by `DeleteTaskCommandHandler`) passes the check.
  _depends on: —_
  _covers: AC-1, AC-2_
