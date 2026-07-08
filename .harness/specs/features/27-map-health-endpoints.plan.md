# Plan: 27 — Map /health/live and /health/ready Endpoints

**Spec:** `.harness/specs/features/27-map-health-endpoints.md`  
**Status:** Ready  
**Created:** 2026-07-08  

> **Note:** Implement in the same branch as #26. Routes without registered probes (and probes without routes) are not independently testable. Close with a single PR covering both issues.

## Tasks

- [ ] **task-1** · agent: `engineer`  
  Add two `app.MapHealthChecks` calls in `Tasks.Api/Program.cs`: `/health/live` with `Predicate = _ => false` (always 200, no I/O) and `/health/ready` filtered to the `"readiness"` tag, using the built-in `System.Text.Json` response writer via `HealthCheckOptions.ResponseWriter` — do NOT add `AspNetCore.HealthChecks.UI.Client`; it is not a transitive dependency and adds unnecessary weight.  
  _depends on: —_  
  _covers: AC-1, AC-2, AC-3, AC-4_
