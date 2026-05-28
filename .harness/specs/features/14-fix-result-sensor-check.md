# Spec: Fix architecture sensor Result check

**Issue:** #14  
**Author:** @dilterporto  
**Status:** Implementing  
**Agent:** engineer  

---

## Context

The architecture fitness sensor checks that all command handlers use the Result pattern. The current grep (`Result<`) requires the generic form and incorrectly flags `DeleteTaskCommandHandler`, which returns non-generic `Result` — the correct type for a command that deletes without returning a value.

This must be fixed before the sensor is added to CI (issue #15), otherwise the CI job would produce a false-positive failure on every run.

---

## Specification

### What will be built

Update the grep command in `.harness/sensors/architecture-fitness.md` from `Result<` to `Result` (without the generic bracket) so non-generic `Result` usages pass the check.

### Inputs and outputs

No code changes. Only the sensor documentation is updated.

### Behavior

- **Must** update the grep command from `grep -rL "Result<"` to `grep -rL "Result"`
- **Must** verify the updated check passes against all current handlers
- **Must not** change any handler implementations

---

## Acceptance Criteria

- [ ] AC-1: `grep -rL "Result" src/Tasks.Application/UseCases/ --include="*Handler.cs"` → empty (no output)
- [ ] AC-2: `.harness/sensors/architecture-fitness.md` contains `grep -rL "Result"` (not `Result<`)

---

## Technical Constraints

- Only `.harness/sensors/architecture-fitness.md` is modified

---

## Out of Scope

- Changing `DeleteTaskCommandHandler` or any other handler
- Adding new sensor checks

---

## Sensors

- [ ] Manual: run the updated check, confirm empty output
