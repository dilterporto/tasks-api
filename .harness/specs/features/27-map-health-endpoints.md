# Spec: Map /health/live and /health/ready Endpoints

**Issue:** #27  
**Author:** @dilterporto  
**Status:** Implementing  
**Agent:** engineer  

---

## Context

Part of #25. With health check probes registered (issue #26), the endpoints must be mapped so ECS and ops tooling can reach them over HTTP. Mapping belongs in `Tasks.Api/Program.cs` using ASP.NET Core's built-in `MapHealthChecks` API.

*Depends on: #26 (probes must be registered before endpoints can be mapped)*

---

## Specification

### What will be built

Two route mappings added to `Tasks.Api/Program.cs`:

- `GET /health/live` → liveness check (no probes, always returns 200 if process is up)
- `GET /health/ready` → readiness check (probes tagged `"readiness"`, JSON response body)

### Inputs and outputs

```
GET /health/live
  Response: 200 OK (plain)

GET /health/ready  (all healthy)
  Response: 200 OK
  Body: { "status": "Healthy", "results": { ... } }

GET /health/ready  (any probe unhealthy)
  Response: 503 Service Unavailable
  Body: { "status": "Unhealthy", "results": { ... } }
```

### Behavior

- **Must** call `app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false })` — zero probes, always alive
- **Must** call `app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = r => r.Tags.Contains("readiness"), ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse })` — or equivalent standard JSON writer
- **Must** allow anonymous access to both routes (`.AllowAnonymous()` if auth middleware is present)
- **Must not** add route authorization requirements to either endpoint
- **Must not** register probes inside `Program.cs` (that belongs in `Tasks.DependencyInjection`)

---

## Acceptance Criteria

- [ ] AC-1: `GET /health/live` returns `200 OK`
- [ ] AC-2: `GET /health/ready` returns `200 OK` with JSON body when all probes pass
- [ ] AC-3: Both routes are accessible without authentication
- [ ] AC-4: `dotnet build Tasks.sln` passes

---

## Technical Constraints

- Layers affected: `Tasks.Api` only (`Program.cs`)
- Use `Microsoft.AspNetCore.Diagnostics.HealthChecks` (built-in, no extra package needed) for `MapHealthChecks`
- For JSON response writer on `/health/ready`, use `AspNetCore.HealthChecks.UI.Client` (`UIResponseWriter`) or the built-in `HealthCheckOptions.ResponseWriter` with `JsonSerializer`; whichever is already a transitive dependency — avoid adding a new package if unnecessary
- **Must not** add health check route registration to `FastEndpoints` endpoint classes; use `app.MapHealthChecks` directly

---

## Out of Scope

- Probe registration (covered in #26)
- Integration tests (covered in #28)
- CI smoke gate (covered in #29)
- Custom response body format beyond ASP.NET Core health check JSON

---

## Sensors

- [ ] Convention: `grep "MapHealthChecks" src/Tasks.Api/Program.cs` → two matches (`/health/live`, `/health/ready`)
- [ ] Convention: `grep "AllowAnonymous\|RequireAuthorization" src/Tasks.Api/Program.cs` → live and ready routes are not behind auth
- [ ] Build: `dotnet build Tasks.sln` → exit 0
