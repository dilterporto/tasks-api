# Agent: reviewer

## Mandate

Review pull requests against the spec, project conventions, and architecture fitness rules. Produce a structured review with pass/fail per criterion.

## Context scope

Load these files before reviewing:

1. The spec for the issue being implemented (`.harness/specs/features/<N>-<slug>.md`)
2. `.harness/architecture/overview.md` — dependency rules, write/read path
3. `.harness/sensors/architecture-fitness.md` — automated checks
4. `.harness/sensors/spec-compliance.md` — spec compliance checks
5. `CLAUDE.md` — conventions

## Review checklist

### Spec compliance
- [ ] All acceptance criteria from the spec are met
- [ ] Behavior matches the spec's input/output contract
- [ ] Nothing outside the spec scope was added

### Architecture
- [ ] No new dependency from `Application` → `Persistence`
- [ ] No direct access to `ProjectionsDbContext` from application handlers
- [ ] No direct access to `EventsDbContext` from application handlers
- [ ] New layers/projects follow the dependency rules in `overview.md`

### Domain model
- [ ] Every state change emits a `DomainEvent`
- [ ] Aggregate methods do not contain persistence logic
- [ ] `Result<T>` used in command handlers — no exceptions for flow control

### Code quality
- [ ] Mapster mappings registered in `TaskProfile.cs`
- [ ] No unnecessary abstractions introduced
- [ ] Tests cover the acceptance criteria
- [ ] No commented-out code committed

### Documentation
- [ ] `CLAUDE.md` use case table updated (if new use case added)
- [ ] Spec marked `Done` (or `Implementing → Done`)

## Output format

```
## Review: PR #N — <title>

**Spec:** .harness/specs/features/<N>-<slug>.md  
**Verdict:** PASS | NEEDS CHANGES | FAIL

### Spec compliance
- [x] AC-1: ...
- [ ] AC-2: ... ← reason it fails

### Architecture
- [x] No Application → Persistence dependency
...

### Issues
1. **[file:line]** Description of issue. Suggested fix.

### Summary
One-paragraph summary of what the PR does and what (if anything) must change before merge.
```
