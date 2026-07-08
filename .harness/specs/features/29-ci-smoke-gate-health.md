# Spec: CI Smoke Gate for /health/ready

**Issue:** #29  
**Author:** @dilterporto  
**Status:** Done  
**Agent:** engineer  

---

## Context

Part of #25. After the full stack starts in CI (API + PostgreSQL + Redis), there is no gate to confirm the API is actually ready before integration tests run. Tests that execute against a not-yet-ready API produce misleading failures.

A smoke gate step using `curl` against `/health/ready` gives CI a deterministic signal: only proceed to integration tests when the stack is confirmed healthy.

*Depends on: #27 (endpoint must exist before CI can curl it)*

---

## Specification

### What will be built

A new CI step in `.github/workflows/ci.yml` that runs after infrastructure starts and before integration tests execute:

```bash
curl -sf http://localhost:5006/health/ready || exit 1
```

### Inputs and outputs

```
Input:  Running API on localhost:5006 with PostgreSQL and Redis up
Output: Step exits 0 (healthy) or exits 1 (unhealthy → CI fails)
```

### Behavior

- **Must** add the smoke gate step after the infrastructure start step and before the `dotnet test` step
- **Must** use `curl -sf` (silent + fail-on-HTTP-error) so any non-2xx response fails the step
- **Must** target `http://localhost:5006/health/ready` (port consistent with `docker-compose.yml` and `dotnet run` default)
- **Must** fail the CI job (`|| exit 1`) if the endpoint returns non-200 or is unreachable
- **Should** add a `name:` label to the step: `"Smoke gate: /health/ready"`
- **Must not** change existing build or test steps
- **Must not** add a retry loop — if the stack is not up yet, the infrastructure start step is the problem

---

## Acceptance Criteria

- [ ] AC-1: `.github/workflows/ci.yml` contains a step running `curl -sf http://localhost:5006/health/ready || exit 1`
- [ ] AC-2: The smoke gate step appears after infrastructure start and before `dotnet test`
- [ ] AC-3: CI passes end-to-end when the full stack is healthy
- [ ] AC-4: CI fails at the smoke gate step (not at the test step) when the API is unhealthy

---

## Technical Constraints

- File affected: `.github/workflows/ci.yml` only
- Port `5006` must match the value set in issue #9e3bde4 (dotnet run port alignment) and `docker-compose.yml`
- **Must not** change the existing `dotnet restore`, `dotnet build`, or `dotnet test` steps
- **Must not** add a wait loop or sleep before curl — infrastructure readiness is the responsibility of the start step

---

## Out of Scope

- Liveness check in CI (only readiness is meaningful for gate purposes)
- Retry/backoff logic
- Integration test changes

---

## Sensors

- [ ] Convention: `grep "health/ready" .github/workflows/ci.yml` → matches exactly one step
- [ ] Convention: `grep "curl -sf" .github/workflows/ci.yml` → present
- [ ] CI: workflow passes on a branch with healthy infrastructure
