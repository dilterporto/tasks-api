# Spec: Create infra-fitness sensor and update spec #10 sensors section

**Issue:** #22  
**Author:** @dilterporto  
**Status:** Implementing  
**Agent:** architect  

---

## Context

Issue #10 specifies Terraform infrastructure for ECS Fargate + API Gateway + RDS PostgreSQL. Before the infra-engineer starts, the exit criteria must be defined in verifiable sensor form.

The current sensor ecosystem (`architecture-fitness.md`, `spec-compliance.md`) covers only .NET code invariants. There is no fitness check for Terraform modules — which means an infrastructure PR has no automated verification of security, parameterization, or structural correctness.

Without this sensor, the reviewer must invent checks ad-hoc at PR time, which defeats the spec-driven workflow.

---

## Specification

### What will be built

A new sensor file `.harness/sensors/infra-fitness.md` documenting four groups of Terraform fitness invariants, each with runnable commands. The `## Sensors` section of spec #10 will be updated to reference the concrete checks from this file. Spec #10 will then be marked `Ready`.

### Inputs and outputs

```
Input:  Analyst reads spec #10 requirements and Must/Must not statements
Output: .harness/sensors/infra-fitness.md  — four check groups, each with ≥1 runnable command
        .harness/specs/features/10-terraform-aws-infrastructure-multi-env.md — §Sensors updated
        Spec #10 status → Ready
```

### Behavior

- **Must** cover four check groups: structural validation, security/credentials, module parameterization, pipeline and documentation
- **Must** provide at least one runnable command per check group (grep, terraform CLI, or git command)
- **Must** map each check back to the spec #10 Must/Must not statement it enforces
- **Must** update spec #10 `## Sensors` section with the concrete commands
- **Must** set spec #10 status to `Ready` after the sensors section is updated
- **Must not** implement the CI job that runs the sensors — that is part of #10
- **Must not** create or modify any Terraform files
- **Should** follow the same structure as `architecture-fitness.md`: manual check commands first, note that automated enforcement comes later

---

## Acceptance Criteria

- [ ] AC-1: `.harness/sensors/infra-fitness.md` exists with four labeled check groups
- [ ] AC-2: Each check group has at least one concrete runnable command
- [ ] AC-3: Each check maps to a specific Must/Must not from spec #10
- [ ] AC-4: `## Sensors` section of spec #10 is replaced with the concrete checks from `infra-fitness.md`
- [ ] AC-5: Spec #10 `**Status:**` field is updated to `Ready`

---

## Technical Constraints

- **Scope:** touches only `.harness/sensors/` and `.harness/specs/features/10-*.md` — no source code, no CI files
- **Format:** sensor commands must be copy-pasteable as-is in a shell with the repo root as working directory
- **Naming:** check group names must match the four categories identified in the architect analysis: Structural Validation, Security & Credentials, Module Parameterization, Pipeline & Documentation

---

## Out of Scope

- Implementing the CI job (`infra-sensor` job in `.github/workflows/ci.yml`) — part of #10
- Creating or modifying any Terraform files — part of #10
- Updating `CLAUDE.md` or `README.md` — part of #10

---

## Sensors

- [ ] File: `.harness/sensors/infra-fitness.md` exists
- [ ] File: contains exactly four `##` check group headings
- [ ] File: each group contains a fenced code block with a runnable command
- [ ] Spec #10: `## Sensors` section contains grep/terraform/git commands (not placeholder text)
- [ ] Spec #10: `**Status:**` line reads `Ready`
