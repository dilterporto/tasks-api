# Spec: [Title]

**Issue:** #N  
**Author:** @  
**Status:** Draft | Ready | Implementing | Done  
**Agent:** engineer | infra-engineer  

---

## Context

Why does this need to exist? What problem does it solve? What happens if we don't do it?

---

## Specification

### What will be built

Clear description of the feature or change. Write in terms of behavior, not implementation.

### Inputs and outputs

What does the system receive? What does it produce?

```
Input:  { ... }
Output: { ... }
```

### Behavior

Describe what the system must do step by step. Use "must", "must not", "should" for clarity.

- **Must** ...
- **Must not** ...
- **Should** ...

---

## Acceptance Criteria

Each criterion must be independently verifiable — ideally covered by a test.

- [ ] AC-1: ...
- [ ] AC-2: ...
- [ ] AC-3: ...

---

## Technical Constraints

Implementation boundaries the engineer must respect.

- Layer: which projects are affected (e.g., Domain only, Application + Persistence)
- Patterns: which patterns must be followed (e.g., must emit a DomainEvent, must use Result<T>)
- Must not: what is explicitly out of bounds (e.g., must not access ProjectionsDbContext directly from handler)

---

## Out of Scope

What is explicitly NOT part of this spec.

- ...

---

## Sensors

How to verify this spec was correctly implemented.

- [ ] Test: `<TestClassName>_<Scenario>` passes
- [ ] Architecture: no new dependencies introduced from Application → Persistence directly
- [ ] Convention: handler follows Result<T> chain pattern
- [ ] Docs: CLAUDE.md use case table updated (if new use case)
