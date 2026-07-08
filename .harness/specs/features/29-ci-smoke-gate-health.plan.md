# Plan: 29 — CI Smoke Gate for /health/ready

**Spec:** `.harness/specs/features/29-ci-smoke-gate-health.md`  
**Status:** Ready  
**Created:** 2026-07-08  

## Context notes

- The integration tests from #28 do **not** run in CI (no infra services in the `build-and-test` job). This smoke gate is the CI-level verification for the health check endpoints — it depends on the full stack being started by an existing infrastructure step.
- The CI workflow currently has no step that starts the API + infra. If no such step exists at the time this is implemented, the engineer must also add a step to start `docker-compose` (or equivalent) before the smoke gate.

## Tasks

- [ ] **task-1** · agent: `engineer`  
  Insert a step named `"Smoke gate: /health/ready"` running `curl -sf http://localhost:5006/health/ready || exit 1` in `.github/workflows/ci.yml`, after any infrastructure/API start step and before `dotnet test`; if no infra start step exists in the job, add one that starts the full stack (API + PostgreSQL + Redis) using `docker-compose up -d` and waits for the process to bind the port before the curl executes.  
  _depends on: —_  
  _covers: AC-1, AC-2, AC-3, AC-4_
