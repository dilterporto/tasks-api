# Agents

Specialized agents for this repository. Each agent has a defined mandate, context scope, and constraints. Always load the relevant agent file before starting a task.

## Registry

| Agent | File | Mandate | When to use |
|-------|------|---------|-------------|
| `architect` | [architect.md](architect.md) | Design decisions, architecture review, ADR authoring | New patterns, cross-cutting changes, dependency additions |
| `planner` | [planner.md](planner.md) | Decompose a spec into tasks and allocate each to the right agent | After spec is `Ready`, before any implementation starts |
| `engineer` | [engineer.md](engineer.md) | Implement use cases and features following DDD/CQRS/Event Sourcing patterns | New use cases, bug fixes, refactoring in `src/` |
| `infra-engineer` | [infra-engineer.md](infra-engineer.md) | Terraform infrastructure, AWS resources, multi-environment config | Everything in `infra/` |
| `reviewer` | [reviewer.md](reviewer.md) | Code review against spec, conventions and architecture fitness | PR review |

## How to invoke an agent

At the start of a task, state which agent is active and load its file:

```
Agent: engineer
Task: implement issue #12
Spec: .harness/specs/features/12-complete-task.md
```

The agent reads its own mandate file, the relevant spec, and the guides referenced in the mandate before producing any output.

## Spec-first rule

No `engineer` or `infra-engineer` task starts without a spec file in `.harness/specs/features/`. If the spec does not exist, the `architect` agent writes it first.
