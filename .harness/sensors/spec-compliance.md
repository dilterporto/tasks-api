# Sensor: Spec Compliance

These checks verify that an implementation matches its spec before a PR is merged.

## How to use

When reviewing a PR, load the corresponding spec from `.harness/specs/features/` and run these checks.

---

## Acceptance criteria coverage

For each criterion in the spec's `## Acceptance Criteria` section:

1. Identify the test that covers it: `<TestClassName>_<Scenario>`
2. Run that test: `dotnet test tests/Tasks.Tests/ --filter "FullyQualifiedName~<ClassName>"`
3. Verify it passes

If an acceptance criterion has no corresponding test, the PR fails this sensor.

---

## Behavior conformance

1. Compare the spec's `### Behavior` section (Must/Must not/Should statements) against the implementation
2. For each **Must** statement, verify the code enforces it unconditionally
3. For each **Must not** statement, verify the code never does it
4. For each **Should** statement, verify either a) it is implemented, or b) there is a documented reason it was deferred

---

## Input/output contract

1. Compare the spec's `### Inputs and outputs` section against:
   - The command/query record definition
   - The response DTO
   - The FastEndpoints request/response types
2. Field names, types, and required/optional semantics must match the spec

---

## Scope compliance

Review the diff against the spec's `## Out of Scope` section. If any change falls under "Out of Scope", it must be removed or moved to a new issue.

---

## Checklist for reviewers

```
Spec: .harness/specs/features/<N>-<slug>.md

[ ] All AC items have a passing test
[ ] All Must statements are enforced in the implementation
[ ] All Must not statements have no counter-examples in the diff
[ ] Input/output types match the spec contract
[ ] No out-of-scope changes in the diff
[ ] Spec status updated to Done
```

---

## Spec status lifecycle

| Status | Meaning |
|--------|---------|
| `Draft` | Being written by architect |
| `Ready` | Approved by architect, engineer can start |
| `Implementing` | Engineer branch open |
| `Done` | PR merged, all AC verified |
