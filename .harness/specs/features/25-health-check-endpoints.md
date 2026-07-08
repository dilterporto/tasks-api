# Spec: Add Health Check Endpoints

**Issue:** #25  
**Author:** @dilterporto  
**Status:** Ready  
**Agent:** engineer  

---

## Context

The API has no mechanism to signal its own health to the load balancer or ECS orchestrator. Without health check endpoints:

- ECS cannot determine if a container is ready to receive traffic (readiness probe)
- ECS cannot restart an unhealthy container (liveness probe)
- Ops has no quick way to confirm PostgreSQL and Redis are reachable after a deploy

This is also a prerequisite for the CI smoke gate (issue #29), which verifies the full stack before integration tests run.

---

## Specification

### What will be built

Two HTTP endpoints that expose the runtime health of the service:

- `GET /health/live` — liveness probe: confirms the process is up, no I/O
- `GET /health/ready` — readiness probe: confirms PostgreSQL (event store + projections) and Redis are reachable

### Inputs and outputs

```
GET /health/live
  Input:  (no body, no auth)
  Output: 200 OK

GET /health/ready
  Input:  (no body, no auth)
  Output (healthy):   200 OK  + JSON (ASP.NET Core health check format)
  Output (unhealthy): 503 Service Unavailable + JSON with failed component details
```

ASP.NET Core health check JSON format:
```json
{
  "status": "Healthy",
  "results": {
    "postgresql-events":      { "status": "Healthy" },
    "postgresql-projections": { "status": "Healthy" },
    "redis":                  { "status": "Healthy" }
  }
}
```

### Behavior

- **Must** register three named health check probes: `postgresql-events`, `postgresql-projections`, `redis`
- **Must** tag `postgresql-events` and `postgresql-projections` with `"readiness"`
- **Must** tag `redis` with `"readiness"`
- **Must** expose `GET /health/live` mapped to checks tagged `"liveness"` (no probes → always 200)
- **Must** expose `GET /health/ready` mapped to checks tagged `"readiness"` with JSON response writer
- **Must** return `200 OK` on `/health/ready` only when all three probes succeed
- **Must** return `503 Service Unavailable` on `/health/ready` when any probe fails
- **Must not** require authentication on either endpoint
- **Must not** add health check logic inside domain, application, or persistence layers

---

## Acceptance Criteria

- [ ] AC-1: `GET /health/live` returns `200 OK` when the process is running
- [ ] AC-2: `GET /health/ready` returns `200 OK` when PostgreSQL and Redis are reachable
- [ ] AC-3: `GET /health/ready` returns `503` when PostgreSQL is down
- [ ] AC-4: `GET /health/ready` returns `503` when Redis is down
- [ ] AC-5: Response body follows ASP.NET Core health check JSON format
- [ ] AC-6: Both endpoints require no authentication

---

## Technical Constraints

- Layers affected: `Tasks.Api` (endpoint mapping), `Tasks.DependencyInjection` (probe registration)
- Use `AspNetCore.HealthChecks.NpgSql` and `AspNetCore.HealthChecks.Redis` packages (add to `Directory.Packages.props`)
- Package versions must be added to `Directory.Packages.props`; `.csproj` references must not pin versions
- Health check registration belongs in `Tasks.DependencyInjection.DependencyExtensions`, not in `Program.cs`
- Endpoint mapping belongs in `Tasks.Api/Program.cs`
- **Must not** add health check dependencies to `Tasks.Application`, `Tasks.Domain`, or `Tasks.Persistence`

---

## Out of Scope

- Authentication or authorization on health endpoints
- Custom health check logic beyond NpgSql and Redis probes
- Metrics or Prometheus exposition
- UI dashboard for health status

---

## Sensors

- [ ] Test: `HealthCheckEndpointsTests_Liveness_Returns200` passes
- [ ] Test: `HealthCheckEndpointsTests_Readiness_Returns200_WhenAllHealthy` passes
- [ ] Test: `HealthCheckEndpointsTests_Readiness_Returns503_WhenPostgresDown` passes
- [ ] Test: `HealthCheckEndpointsTests_Readiness_Returns503_WhenRedisDown` passes
- [ ] Architecture: no health check `using` in `src/Tasks.Application/` or `src/Tasks.Domain/`
- [ ] Build: `dotnet build Tasks.sln` → exit 0
- [ ] Test: `dotnet test tests/Tasks.Tests/` → exit 0
