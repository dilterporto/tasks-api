# Spec: Register NpgSql and Redis Health Check Probes

**Issue:** #26  
**Author:** @dilterporto  
**Status:** Implementing  
**Agent:** engineer  

---

## Context

Part of #25. Before the health check endpoints can be mapped in `Tasks.Api`, the three health check probes must be registered in the DI container. Registration belongs in `Tasks.DependencyInjection` to keep `Program.cs` thin and consistent with how all other services are wired.

---

## Specification

### What will be built

Register three named health check probes in `Tasks.DependencyInjection/DependencyExtensions.cs`:

- `postgresql-events` — checks the event store connection string, tagged `"readiness"`
- `postgresql-projections` — checks the projections connection string, tagged `"readiness"`
- `redis` — checks the Redis connection string, tagged `"readiness"`

Add the required packages to `Directory.Packages.props` and reference them from `Tasks.DependencyInjection.csproj`.

### Inputs and outputs

```
Input:  IServiceCollection (via AddDependencies extension)
Output: IServiceCollection with three health check probes registered
```

### Behavior

- **Must** add `AspNetCore.HealthChecks.NpgSql` to `Directory.Packages.props`
- **Must** add `AspNetCore.HealthChecks.Redis` to `Directory.Packages.props`
- **Must** reference both packages in `Tasks.DependencyInjection.csproj` (no version pinning)
- **Must** call `services.AddHealthChecks()` and chain `.AddNpgSql(connectionString, name: "postgresql-events", tags: ["readiness"])` and `.AddNpgSql(connectionString, name: "postgresql-projections", tags: ["readiness"])` and `.AddRedis(connectionString, name: "redis", tags: ["readiness"])`
- **Must** read connection strings from `IConfiguration` (consistent with existing pattern in `DependencyExtensions`)
- **Must not** hard-code connection strings
- **Must not** register health checks in `Program.cs`

---

## Acceptance Criteria

- [ ] AC-1: `Directory.Packages.props` includes `AspNetCore.HealthChecks.NpgSql` and `AspNetCore.HealthChecks.Redis`
- [ ] AC-2: `Tasks.DependencyInjection.csproj` references both packages without version attributes
- [ ] AC-3: Three probes registered: `postgresql-events`, `postgresql-projections`, `redis` — all tagged `"readiness"`
- [ ] AC-4: Connection strings are read from `IConfiguration`, not hard-coded
- [ ] AC-5: `dotnet build Tasks.sln` passes

---

## Technical Constraints

- Layers affected: `Tasks.DependencyInjection` only
- Registration must extend the existing `AddDependencies()` method (no new extension methods)
- Package versions in `Directory.Packages.props` only — `.csproj` must use `<PackageReference>` without `Version`
- **Must not** touch `Tasks.Api`, `Tasks.Application`, `Tasks.Domain`, or `Tasks.Persistence`

---

## Out of Scope

- Mapping the endpoints (covered in #27)
- Integration tests (covered in #28)
- Liveness tag or probe (liveness check is zero-I/O; no probe needed)

---

## Sensors

- [ ] Build: `dotnet build Tasks.sln` → exit 0
- [ ] Convention: `grep -r "AddHealthChecks" src/Tasks.DependencyInjection/` finds exactly one call site
- [ ] Convention: `grep -r "postgresql-events\|postgresql-projections\|redis" src/Tasks.DependencyInjection/` matches three probes
- [ ] Convention: no `Version=` attribute in `Tasks.DependencyInjection.csproj` health check references
