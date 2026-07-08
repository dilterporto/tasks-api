# Spec: Integration Tests for Health Check Endpoints

**Issue:** #28  
**Author:** @dilterporto  
**Status:** Ready  
**Agent:** engineer  

---

## Context

Part of #25. The health check endpoints expose runtime infrastructure state. Unit tests cannot meaningfully verify them — integration tests against real (or containerized) dependencies are required to confirm each acceptance criterion from the parent spec.

*Depends on: #27 (endpoints must be mapped before tests can hit them)*

---

## Specification

### What will be built

Integration tests in `tests/Tasks.Tests/` covering AC-1 through AC-6 of issue #25:

| Test | Scenario |
|------|----------|
| Liveness_Returns200 | `/health/live` always returns 200 |
| Readiness_Returns200_WhenAllHealthy | `/health/ready` returns 200 when PostgreSQL + Redis are up |
| Readiness_Returns503_WhenPostgresDown | `/health/ready` returns 503 when PostgreSQL connection fails |
| Readiness_Returns503_WhenRedisDown | `/health/ready` returns 503 when Redis connection fails |

### Inputs and outputs

```
Input:  HTTP GET to /health/live or /health/ready via WebApplicationFactory<Program>
Output: HttpResponseMessage with StatusCode and JSON body
```

### Behavior

- **Must** use `WebApplicationFactory<Program>` (or equivalent test server) consistent with existing integration test setup in the project
- **Must** cover all four scenarios listed above as independent test methods
- **Must** assert `HttpStatusCode.OK` (200) for liveness and healthy readiness
- **Must** assert `HttpStatusCode.ServiceUnavailable` (503) for unhealthy readiness
- **Must** assert response body contains `"status"` field with `"Healthy"` or `"Unhealthy"` for readiness checks
- **Must not** use mocks for infrastructure — tests must hit real or test-containerized PostgreSQL and Redis
- **Should** simulate failure by overriding connection strings with invalid values (unreachable host) via `WebApplicationFactory.WithWebHostBuilder`

---

## Acceptance Criteria

- [ ] AC-1: `HealthCheckEndpointsTests_Liveness_Returns200` passes
- [ ] AC-2: `HealthCheckEndpointsTests_Readiness_Returns200_WhenAllHealthy` passes
- [ ] AC-3: `HealthCheckEndpointsTests_Readiness_Returns503_WhenPostgresDown` passes
- [ ] AC-4: `HealthCheckEndpointsTests_Readiness_Returns503_WhenRedisDown` passes
- [ ] AC-5: Response body for `/health/ready` contains `"status"` key
- [ ] AC-6: Tests are in `tests/Tasks.Tests/` and run with `dotnet test`

---

## Technical Constraints

- Tests must follow the existing pattern in `tasks/Tasks.Tests` (constructor injection, no base classes)
- Infrastructure (PostgreSQL + Redis) must be available during test run — use `docker-compose.dev-env.yml` or configure via environment variable overrides
- To simulate unhealthy state: configure `WebApplicationFactory` to override connection string with a non-routable address (e.g., `host=127.0.0.2`) — do not stop real containers
- **Must not** mock `IHealthCheck` implementations — the point is to test the real probes
- **Must not** add test-only packages that are not already in `Directory.Packages.props`

---

## Out of Scope

- Load or stress testing health endpoints
- Testing specific probe response fields beyond `status`
- CI smoke gate (covered in #29)

---

## Sensors

- [ ] Test: `dotnet test --filter "FullyQualifiedName~HealthCheckEndpointsTests"` → all pass
- [ ] Convention: no `Mock<IHealthCheck>` in health check test files
- [ ] Build: `dotnet build Tasks.sln` → exit 0
