# ADR-004: Trunk-based development

**Status:** Accepted  
**Date:** 2024-06-01

## Context

The project needed a branching strategy that keeps `main` always deployable, reduces merge conflicts from long-lived branches, and integrates cleanly with GitHub Actions CI/CD and GitHub Issues.

## Decision

Adopt trunk-based development:

- `main` is the trunk — always deployable
- All feature work happens on short-lived branches named `type/#N-description` where `N` is the GitHub issue number
- Branches merge to `main` via squash merge only (one commit per feature/fix)
- PRs must include `Closes #N` in the description — this auto-closes the issue on merge
- Direct push to `main` is blocked by branch protection rules
- CI (build + test) must pass before merge is allowed

Branch naming convention:

```
feat/#N-short-description    ← new feature
fix/#N-short-description     ← bug fix
chore/#N-short-description   ← tooling, config, docs
infra/#N-short-description   ← infrastructure
```

## Consequences

**Easier:**
- `main` is always in a known-good state
- Issues are automatically closed on merge — no manual housekeeping
- CI feedback is fast (short-lived branches stay close to main)
- One squash commit per feature keeps `git log` readable

**Harder:**
- Long-running features need to be broken into smaller vertical slices
- Incomplete features need feature flags (not currently implemented)

## Alternatives considered

- **Gitflow**: Long-lived `develop` and `release` branches add coordination overhead with no benefit at this team size
- **GitHub Flow** (PR to main without squash): Works but produces noisy commit history with multiple intermediate commits per feature
