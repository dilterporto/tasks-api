# Plan: 28 — Integration Tests for Health Check Endpoints

**Spec:** `.harness/specs/features/28-integration-tests-health-checks.md`  
**Status:** Ready  
**Created:** 2026-07-08  

## Context notes

- All existing tests in `Tasks.Tests` are **unit tests** using Moq — no `WebApplicationFactory` exists yet.
- `Microsoft.AspNetCore.Mvc.Testing` is **not** in `Directory.Packages.props` and must be added.
- The current CI `build-and-test` job has **no PostgreSQL or Redis service containers** — integration tests that hit real infrastructure will fail in CI as-is.
- Resolution: these tests run locally against `docker-compose.dev-env.yml`. In CI, the `/health/ready` endpoint is verified by the smoke gate in #29, which runs against the real stack. Do not add service containers to CI to support these tests — keep the unit test job fast and infra-free.

## Tasks

- [ ] **task-1** · agent: `engineer`  
  Add `Microsoft.AspNetCore.Mvc.Testing` to `Directory.Packages.props` and reference it in `Tasks.Tests.csproj`; create `HealthCheckEndpointsTests` with four test methods using `WebApplicationFactory<Program>`: liveness 200, readiness 200 when healthy, readiness 503 when the PostgreSQL connection string is overridden to a non-routable address (`host=127.0.0.2`), and readiness 503 when the Redis connection string is overridden similarly — all via `WithWebHostBuilder` configuration override, not by stopping containers.  
  _depends on: —_  
  _covers: AC-1, AC-2, AC-3, AC-4, AC-5, AC-6_
